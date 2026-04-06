using ExperimentPlatformDomain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ExperimentPlatformInfrastructure.Persistence.Configurations
{
    public class ExperimentConfiguration : IEntityTypeConfiguration<Experiment>
    {
        public void Configure(EntityTypeBuilder<Experiment> builder)
        {
            builder.ToTable("Experiments");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Id);

            builder.Property(x => x.Name)
                .IsRequired();

            builder.Property(x => x.IsActive);

            builder.HasMany(x => x.Variants)
                .WithOne()
                .HasForeignKey(nameof(Variant.ExperimentId))
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
