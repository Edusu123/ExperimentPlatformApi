using ExperimentPlatformApplication.Events;
using Microsoft.AspNetCore.Mvc;

namespace ExperimentPlatform.Controllers
{
    [ApiController]
    [Route("api/experiments/{experimentId:guid}/[controller]")]
    public class EventsController(TrackEventHandler trackEvent) : ControllerBase
    {
        private readonly TrackEventHandler _trackEvent = trackEvent;

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status202Accepted)]
        public async Task<IActionResult> Track(
            Guid experimentId,
            [FromBody] TrackEventRequest request)
        {
            await _trackEvent.Handle(experimentId, request.UserId, request.Type);
            return Accepted();
        }
    }

    public sealed record TrackEventRequest(string UserId, string Type);
}
