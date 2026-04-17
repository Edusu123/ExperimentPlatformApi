namespace ExperimentPlatformInfrastructure.Background;

public class RabbitMqSettings
{
    public const string SectionName = "RabbitMQ";

    public string HostName { get; set; } = "localhost";
    public int Port { get; set; } = 5672;
    public string UserName { get; set; } = "guest";
    public string Password { get; set; } = "guest";
    public string QueueName { get; set; } = "experiment-events";
    public string ExchangeName { get; set; } = "experiment-exchange";
    public string RoutingKey { get; set; } = "event.tracked";
}