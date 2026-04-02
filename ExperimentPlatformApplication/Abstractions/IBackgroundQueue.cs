namespace ExperimentPlatformApplication.Abstractions
{
    public interface IBackgroundQueue
    {
        void Enqueue(Func<CancellationToken, Task> workItem);
        Task<object?> DequeueAsync(CancellationToken cancellationToken);
    }
}
