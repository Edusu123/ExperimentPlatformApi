namespace ExperimentPlatformApplication.Experiments.CreateExperiment 
{
    public record CreateExperimentCommand(
        string Name,
        List<CreateVariantDto> Variants
    );

    public record CreateVariantDto(
        string Name,
        int Weight
    );
}
