using ExperimentPlatformDomain.Entities;
using ExperimentPlatformDomain.ValueObjects;
using ExperimentPlatformInfrastructure.Persistence.Converters;
using Microsoft.EntityFrameworkCore;

namespace ExperimentPlatformInfrastructure.Persistence
{
    public class ExperimentDbContext(DbContextOptions<ExperimentDbContext> options) : DbContext(options)
    {
        public DbSet<Experiment> Experiments => Set<Experiment>();
        public DbSet<Event> Events => Set<Event>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(ExperimentDbContext).Assembly);
            base.OnModelCreating(modelBuilder);
        }

        protected override void ConfigureConventions(ModelConfigurationBuilder builder)
        {
            builder.Properties<UserId>().HaveConversion<UserIdConverter>();

            base.ConfigureConventions(builder);
        }
    }
}
