using ExperimentPlatformDomain.Entities;

namespace ExperimentPlatformApplication.Abstractions
{
    public interface IBackgroundQueue
    {
        ValueTask EnqueueAsync(Event evt, Func<IServiceProvider, CancellationToken, Task> workItem);
        ValueTask<Func<IServiceProvider, CancellationToken, Task>> DequeueAsync(CancellationToken ct);
    }
}
