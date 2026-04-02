namespace ExperimentPlatformApplication.Abstractions
{
    public interface IBackgroundQueue
    {
        ValueTask EnqueueAsync(Func<IServiceProvider, CancellationToken, Task> workItem);
        ValueTask<Func<IServiceProvider, CancellationToken, Task>> DequeueAsync(CancellationToken ct);
    }
}
