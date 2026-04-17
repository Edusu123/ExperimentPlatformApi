using ExperimentPlatformApplication.Abstractions;
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

    public RabbitMqBackgroundQueue(IOptions<RabbitMqSettings> settings)
    {
        _settings = settings.Value;
        _localQueue = Channel.CreateUnbounded<Func<IServiceProvider, CancellationToken, Task>>();

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

    public async ValueTask EnqueueAsync(Func<IServiceProvider, CancellationToken, Task> workItem)
    {
        // Serialize the work item metadata (simplified approach)
        // In production, you'd serialize actual event data
        var message = JsonSerializer.Serialize(new
        {
            Timestamp = DateTime.UtcNow,
            Type = "EventTracking"
        });

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

        // Also queue locally for processing
        await _localQueue.Writer.WriteAsync(workItem);
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

            // Process message (acknowledge it)
            await _channel.BasicAckAsync(deliveryTag: ea.DeliveryTag, multiple: false);
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