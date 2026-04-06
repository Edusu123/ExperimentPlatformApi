using ExperimentPlatformDomain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ExperimentPlatformInfrastructure.Persistence.Configurations
{
    public class EventConfiguration : IEntityTypeConfiguration<Event>
    {
        public void Configure(EntityTypeBuilder<Event> builder)
        {
            builder.ToTable("Events");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.UserId);

            builder.Property(x => x.ExperimentId);

            builder.Property(x => x.Type);

            builder.Property(x => x.CreatedAt);

            builder.HasIndex(x => new { x.ExperimentId, x.UserId });
        }
    }
}
