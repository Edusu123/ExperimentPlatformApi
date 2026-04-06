using ExperimentPlatformApplication.Experiments.AssignVariant;
using ExperimentPlatformApplication.Experiments.CreateExperiment;
using Microsoft.AspNetCore.Mvc;

namespace ExperimentPlatform.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ExperimentsController(
        CreateExperimentHandler createExperiment,
        AssignVariantHandler assignVariant) : ControllerBase
    {
        private readonly CreateExperimentHandler _createExperiment = createExperiment;
        private readonly AssignVariantHandler _assignVariant = assignVariant;

        [HttpPost]
        [ProducesResponseType(typeof(CreateExperimentResponse), StatusCodes.Status201Created)]
        public async Task<ActionResult<CreateExperimentResponse>> Create(
            [FromBody] CreateExperimentCommand command,
            CancellationToken cancellationToken)
        {
            var id = await _createExperiment.Handle(command, cancellationToken);
            return Created($"/api/experiments/{id}", new CreateExperimentResponse(id));
        }

        [HttpPost("{experimentId:guid}/assign")]
        [ProducesResponseType(typeof(AssignVariantResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<AssignVariantResponse>> AssignVariant(
            Guid experimentId,
            [FromBody] AssignVariantRequest request,
            CancellationToken cancellationToken)
        {
            try
            {
                var variant = await _assignVariant.Handle(
                    new AssignVariantCommand(experimentId, request.UserId),
                    cancellationToken);

                return Ok(new AssignVariantResponse(variant.Id, variant.Name));
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(
                    new ProblemDetails
                    {
                        Title = "Assignment failed",
                        Detail = ex.Message,
                        Status = StatusCodes.Status400BadRequest
                    });
            }
        }
    }

    public sealed record CreateExperimentResponse(Guid Id);

    public sealed record AssignVariantRequest(string UserId);

    public sealed record AssignVariantResponse(Guid VariantId, string Name);
}
