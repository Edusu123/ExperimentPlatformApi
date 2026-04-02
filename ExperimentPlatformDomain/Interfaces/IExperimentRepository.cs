using ExperimentPlatformDomain.Entities;

namespace ExperimentPlatformDomain.Interfaces
{
    public interface IExperimentRepository
    {
        Task<Experiment?> GetAsync(Guid id, CancellationToken ct);
        Task AddAsync(Experiment experiment, CancellationToken ct);
        Task UpdateAsync(Experiment experiment, CancellationToken ct);
        Task SaveChangesAsync(CancellationToken ct);
    }
}
