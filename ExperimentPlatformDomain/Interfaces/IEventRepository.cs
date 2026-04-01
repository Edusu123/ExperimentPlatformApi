using ExperimentPlatformDomain.Entities;

namespace ExperimentPlatformDomain.Interfaces
{
    public interface IEventRepository
    {
        Task Add(Event _event);
    }
}
