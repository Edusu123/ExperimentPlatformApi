namespace ExperimentPlatformApplication.Experiments.AssignVariant
{
    public record AssignVariantCommand(
        Guid ExperimentId,
        string UserId
    );
}
