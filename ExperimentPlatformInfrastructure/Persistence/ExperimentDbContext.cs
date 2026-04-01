using ExperimentPlatformDomain.Entities;
using Microsoft.EntityFrameworkCore;

namespace ExperimentPlatformInfrastructure.Persistence
{
    public class ExperimentDbContext(DbContextOptions options) : DbContext(options)
    {
        public DbSet<Experiment> Experiments => Set<Experiment>();
        public DbSet<Event> Events => Set<Event>();
    }
}
