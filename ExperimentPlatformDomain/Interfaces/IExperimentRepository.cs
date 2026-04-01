using ExperimentPlatformDomain.Entities;

namespace ExperimentPlatformDomain.Interfaces
{
    public interface IExperimentRepository
    {
        Task<Experiment?> GetByIdAsync(Guid id, CancellationToken ct);
        Task AddAsync(Experiment experiment, CancellationToken ct);
        Task UpdateAsync(Experiment experiment, CancellationToken ct);
        Task SaveChangesAsync(CancellationToken ct);
    }
}
