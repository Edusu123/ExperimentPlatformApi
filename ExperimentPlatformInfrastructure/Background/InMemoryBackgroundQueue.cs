using ExperimentPlatformApplication.Abstractions;
using System.Threading.Channels;

namespace ExperimentPlatformInfrastructure.Background
{
    public class InMemoryBackgroundQueue : IBackgroundQueue
    {
        private readonly Channel<Func<CancellationToken, Task>> _queue = Channel.CreateUnbounded<Func<CancellationToken, Task>>();

        public async Task<object?> DequeueAsync(CancellationToken cancellationToken)
        {
            return await _queue.Reader.ReadAsync(cancellationToken);
        }

        public async void Enqueue(Func<CancellationToken, Task> workItem)
        {
            await _queue.Writer.WriteAsync(workItem);
        }
    }
}
