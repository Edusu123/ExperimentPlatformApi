using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using ExperimentPlatformApplication.Abstractions;
using ExperimentPlatformDomain.Entities;
using ExperimentPlatformDomain.Interfaces;

namespace ExperimentPlatformApplication.Events
{
    public class TrackEventHandler(IBackgroundQueue queue, ILogger<TrackEventHandler> logger)
    {
        private readonly IBackgroundQueue _queue = queue;
        private readonly ILogger<TrackEventHandler> _logger = logger;

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

            _logger.LogInformation("Enqueueing event: Id={EventId}, ExperimentId={ExperimentId}, UserId={UserId}, Type={Type}", 
                evt.Id, evt.ExperimentId, evt.UserId, evt.Type);

            await _queue.EnqueueAsync(evt, async (sp, ct) =>
            {
                var repo = sp.GetRequiredService<IEventRepository>();

                await repo.AddAsync(evt, ct);
                await repo.SaveChangesAsync(ct);
            });

            _logger.LogInformation("Event {EventId} enqueued successfully", evt.Id);
        }
    }
}
