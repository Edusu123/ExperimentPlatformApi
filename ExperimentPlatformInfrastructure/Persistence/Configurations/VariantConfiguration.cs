using ExperimentPlatformDomain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ExperimentPlatformInfrastructure.Persistence.Configurations
{
    public class VariantConfiguration : IEntityTypeConfiguration<Variant>
    {
        public void Configure(EntityTypeBuilder<Variant> builder)
        {
            builder.ToTable("Variant");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.ExperimentId);

            builder.Property(x => x.Name)
                .IsRequired();

            builder.Property(x => x.Weight);
        }
    }
}
