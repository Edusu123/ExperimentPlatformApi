using ExperimentPlatformDomain.Entities;

namespace ExperimentPlatformDomain.Interfaces
{
    public interface IEventRepository
    {
        Task AddAsync(Event evt, CancellationToken ct);

        Task SaveChangesAsync(CancellationToken ct);
    }
}
