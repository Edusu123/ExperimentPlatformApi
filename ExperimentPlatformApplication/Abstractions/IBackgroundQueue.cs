namespace ExperimentPlatformApplication.Abstractions
{
    public interface IBackgroundQueue
    {
        void Enqueue(object workItem);
        Task<object?> DequeueAsync(CancellationToken cancellationToken);
    }
}
