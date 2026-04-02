using ExperimentPlatformApplication.Abstractions;
using System.Threading.Channels;

namespace ExperimentPlatformInfrastructure.Background
{
    public class InMemoryBackgroundQueue : IBackgroundQueue
    {
        private readonly Channel<Func<IServiceProvider, CancellationToken, Task>> _queue = 
            Channel.CreateUnbounded<Func<IServiceProvider, CancellationToken, Task>>();

        public async ValueTask EnqueueAsync(Func<IServiceProvider, CancellationToken, Task> workItem)
        {
            await _queue.Writer.WriteAsync(workItem);
        }

        public async ValueTask<Func<IServiceProvider, CancellationToken, Task>> DequeueAsync(CancellationToken ct)
        {
            return await _queue.Reader.ReadAsync(ct);
        }
    }
}
