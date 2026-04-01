using ExperimentPlatformDomain.Entities;
using ExperimentPlatformDomain.Interfaces;

namespace ExperimentPlatformApplication.Experiments.CreateExperiment
{
    public class CreateExperimentHandler(IExperimentRepository repository)
    {
        private readonly IExperimentRepository _repository = repository;

        public async Task<Guid> Handle(CreateExperimentCommand command)
        {
            var experiment = new Experiment
            {
                Id = Guid.NewGuid(),
                Name = command.Name,
                IsActive = true,
                Variants = command.Variants
                    .Select(v => new Variant
                    {
                        Id = Guid.NewGuid(),
                        Name = v.Name,
                        Weight = v.Weight
                    })
                    .ToList()
            };

            await _repository.Add(experiment);

            return experiment.Id;
        }
    }
}
