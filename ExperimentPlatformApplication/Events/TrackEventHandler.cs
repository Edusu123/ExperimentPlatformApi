using Microsoft.Extensions.DependencyInjection;
using ExperimentPlatformApplication.Abstractions;
using ExperimentPlatformDomain.Entities;
using ExperimentPlatformDomain.Interfaces;

namespace ExperimentPlatformApplication.Events
{
    public class TrackEventHandler(IBackgroundQueue queue)
    {
        private readonly IBackgroundQueue _queue = queue;

        public async Task Handle(Guid experimentId, string userId, string type)
        {
            var evt = new Event
            {
                Id = Guid.NewGuid(),
                ExperimentId = experimentId,
                UserId = userId,
                Type = type,
                CreatedAt = DateTime.UtcNow
            };

            await _queue.EnqueueAsync(async (sp, ct) =>
            {
                var repo = sp.GetRequiredService<IEventRepository>();

                await repo.AddAsync(evt, ct);
                await repo.SaveChangesAsync(ct);
            });
        }
    }
}
