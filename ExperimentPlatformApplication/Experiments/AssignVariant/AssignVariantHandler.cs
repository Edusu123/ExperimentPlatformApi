using ExperimentPlatformDomain.Entities;
using ExperimentPlatformDomain.Interfaces;

namespace ExperimentPlatformApplication.Experiments.AssignVariant
{
    public class AssignVariantHandler(IExperimentRepository repository)
    {
        private readonly IExperimentRepository _repository = repository;

        public async Task<Variant> Handle(AssignVariantCommand command, CancellationToken ct)
        {
            var experiment = await _repository.GetAsync(command.ExperimentId, ct);

            if (experiment is null || !experiment.IsActive)
                throw new InvalidOperationException("Experiment not active");

            // compute a stable hash for (user, experiment) and make it non-negative
            var rawHash = HashCode.Combine(command.UserId, experiment.Id);
            var hash = rawHash == int.MinValue ? (long)int.MaxValue + 1 : Math.Abs((long)rawHash);

            // total bucket size based on variant weights
            var totalWeight = experiment.Variants.Sum(v => v.Weight);

            if (totalWeight <= 0)
                throw new InvalidOperationException("Experiment has no variants with positive weight");

            // select a position in [0, totalWeight) using the hash
            var bucket = (int)(hash % totalWeight);

            int cumulative = 0;

            foreach (var variant in experiment.Variants)
            {
                cumulative += variant.Weight;

                if (bucket < cumulative)
                    return variant;
            }

            return experiment.Variants.First();
        }
    }
}
