using ExperimentPlatformDomain.Entities;

namespace ExperimentPlatformDomain.Interfaces
{
    public interface IExperimentRepository
    {
        Task<Experiment?> Get(Guid id);
        Task Add(Experiment experiment);
    }
}
