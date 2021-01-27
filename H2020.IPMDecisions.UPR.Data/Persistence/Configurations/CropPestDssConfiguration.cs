using H2020.IPMDecisions.UPR.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace H2020.IPMDecisions.UPR.Data.Persistence.Configurations
{
    internal class CropPestDssConfiguration : IEntityTypeConfiguration<CropPestDss>
    {
        public void Configure(EntityTypeBuilder<CropPestDss> builder)
        {
            builder.HasKey(c => c.Id);

            builder.Property(c => c.Id)
                .ValueGeneratedOnAdd();

            builder.HasIndex(
                    c => new
                    {
                        c.CropPestId,
                        c.DssId,
                        c.DssModelId
                    })
                .IsUnique();

            builder.Property(c => c.DssId)
                .IsRequired();

            builder.Property(c => c.DssName)
                .IsRequired();

            builder.Property(c => c.DssModelId)
                .IsRequired();

            builder.Property(c => c.DssModelId)
                .IsRequired();

            builder.HasOne<CropPest>(c => c.CropPest)
                .WithMany(cp => cp.CropPestDsses)
                .HasForeignKey(c => c.CropPestId)
                .HasConstraintName("FK_CropPest_CropPestDss")
                .OnDelete(DeleteBehavior.NoAction)
                .IsRequired();
        }
    }
}