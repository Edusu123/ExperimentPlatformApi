namespace ExperimentPlatformApplication.Abstractions;

public class QueueSettings
{
    public const string SectionName = "QueueSettings";

    public string Type { get; set; } = "InMemory";

    public QueueType GetQueueType()
    {
        return Type.ToLowerInvariant() switch
        {
            "inmemory" => QueueType.InMemory,
            "rabbitmq" => QueueType.RabbitMQ,
            _ => QueueType.InMemory
        };
    }
}