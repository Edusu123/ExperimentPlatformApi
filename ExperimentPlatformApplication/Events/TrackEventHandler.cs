using ExperimentPlatformApplication.Abstractions;
using ExperimentPlatformDomain.Entities;

namespace ExperimentPlatformApplication.Events
{
    public class TrackEventHandler(IBackgroundQueue queue)
    {
        private readonly IBackgroundQueue _queue = queue;

        public Task Handle(Guid experimentId, string userId, string type)
        {
            var ev = new Event
            {
                Id = Guid.NewGuid(),
                ExperimentId = experimentId,
                UserId = userId,
                Type = type,
                CreatedAt = DateTime.UtcNow
            };

            _queue.Enqueue(ev);

            return Task.CompletedTask;
        }
    }
}
