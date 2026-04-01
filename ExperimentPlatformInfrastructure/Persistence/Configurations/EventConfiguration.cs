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

            builder.Property(x => x.ExperimentId)
                .HasColumnName("experiment_id");

            //builder.Property(x => x.Variant)
            //    .HasMaxLength(50)
            //    .HasColumnName("variant");

            builder.Property(x => x.Type);

            builder.Property(x => x.CreatedAt)
                .HasColumnName("created_at");

            builder.HasIndex(x => new { x.ExperimentId, x.UserId });
        }
    }
}
