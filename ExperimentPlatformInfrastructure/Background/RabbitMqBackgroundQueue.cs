using ExperimentPlatformApplication.Abstractions;
using ExperimentPlatformDomain.Entities;
using ExperimentPlatformDomain.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;
using System.Threading.Channels;

namespace ExperimentPlatformInfrastructure.Background;

public class RabbitMqBackgroundQueue : IBackgroundQueue, IDisposable
{
    private readonly RabbitMqSettings _settings;
    private readonly IConnection _connection;
    private readonly IChannel _channel;
    private readonly Channel<Func<IServiceProvider, CancellationToken, Task>> _localQueue;
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<RabbitMqBackgroundQueue> _logger;

    public RabbitMqBackgroundQueue(IOptions<RabbitMqSettings> settings, IServiceProvider serviceProvider, ILogger<RabbitMqBackgroundQueue> logger)
    {
        _settings = settings.Value;
        _serviceProvider = serviceProvider;
        _logger = logger;
        _localQueue = Channel.CreateUnbounded<Func<IServiceProvider, CancellationToken, Task>>();

        _logger.LogInformation("Initializing RabbitMQ Background Queue with HostName={HostName}, Port={Port}", "rabbitmq", _settings.Port);

        // Create connection factory
        var factory = new ConnectionFactory
        {
            HostName = _settings.HostName,
            Port = _settings.Port,
            UserName = _settings.UserName,
            Password = _settings.Password
        };

        // Establish connection and channel
        _connection = factory.CreateConnectionAsync().GetAwaiter().GetResult();
        _channel = _connection.CreateChannelAsync().GetAwaiter().GetResult();

        // Declare exchange and queue
        _channel.ExchangeDeclareAsync(
            exchange: _settings.ExchangeName,
            type: ExchangeType.Direct,
            durable: true,
            autoDelete: false
        ).GetAwaiter().GetResult();

        _channel.QueueDeclareAsync(
            queue: _settings.QueueName,
            durable: true,
            exclusive: false,
            autoDelete: false
        ).GetAwaiter().GetResult();

        _channel.QueueBindAsync(
            queue: _settings.QueueName,
            exchange: _settings.ExchangeName,
            routingKey: _settings.RoutingKey
        ).GetAwaiter().GetResult();

        // Start consuming messages
        StartConsuming();
    }

    public async ValueTask EnqueueAsync(Event evt, Func<IServiceProvider, CancellationToken, Task> workItem)
    {
        // Serialize the event
        var message = JsonSerializer.Serialize(evt);

        var body = Encoding.UTF8.GetBytes(message);

        var properties = new BasicProperties
        {
            Persistent = true,
            DeliveryMode = DeliveryModes.Persistent
        };

        await _channel.BasicPublishAsync(
            exchange: _settings.ExchangeName,
            routingKey: _settings.RoutingKey,
            mandatory: true,
            basicProperties: properties,
            body: body
        );

        // Note: We don't queue locally when using RabbitMQ because the consumer handles processing
    }

    public async ValueTask<Func<IServiceProvider, CancellationToken, Task>> DequeueAsync(CancellationToken ct)
    {
        return await _localQueue.Reader.ReadAsync(ct);
    }

    private void StartConsuming()
    {
        var consumer = new AsyncEventingBasicConsumer(_channel);

        consumer.ReceivedAsync += async (model, ea) =>
        {
            var body = ea.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);

            try
            {
                // Deserialize the event
                var evt = JsonSerializer.Deserialize<Event>(message);

                if (evt != null)
                {
                    _logger.LogInformation("RabbitMQ consumer received event: Id={EventId}, ExperimentId={ExperimentId}, UserId={UserId}", 
                        evt.Id, evt.ExperimentId, evt.UserId);

                    // Create a new scope and insert the event into the database
                    using (var scope = _serviceProvider.CreateScope())
                    {
                        var repo = scope.ServiceProvider.GetRequiredService<IEventRepository>();

                        await repo.AddAsync(evt, CancellationToken.None);
                        await repo.SaveChangesAsync(CancellationToken.None);

                        _logger.LogInformation("Event {EventId} saved successfully via RabbitMQ consumer", evt.Id);
                    }
                }

                // Acknowledge the message after successful processing
                await _channel.BasicAckAsync(deliveryTag: ea.DeliveryTag, multiple: false);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing RabbitMQ message");
                // If processing fails, reject the message (could implement retry logic here)
                await _channel.BasicNackAsync(deliveryTag: ea.DeliveryTag, multiple: false, requeue: true);
            }
        };

        _channel.BasicConsumeAsync(
            queue: _settings.QueueName,
            autoAck: false,
            consumer: consumer
        ).GetAwaiter().GetResult();
    }

    public void Dispose()
    {
        _channel?.Dispose();
        _connection?.Dispose();
        GC.SuppressFinalize(this);
    }
}