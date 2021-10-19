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
                        c.DssVersion,
                        c.DssModelId,
                        c.DssModelVersion,
                        c.DssExecutionType
                    })
                .IsUnique()
                .HasName("IX_CropPestDss_All");

            builder.Property(c => c.DssId)
                .IsRequired();

            builder.Property(c => c.DssName)
                .IsRequired();

            builder.Property(c => c.DssModelId)
                .IsRequired();

            builder.Property(c => c.DssModelName)
                .IsRequired();

            builder.Property(c => c.DssExecutionType)
                .IsRequired();

            builder.Property(c => c.DssModelVersion)
                .IsRequired();

            builder.HasOne<CropPest>(c => c.CropPest)
                .WithMany(cp => cp.CropPestDsses)
                .HasForeignKey(c => c.CropPestId)
                .HasConstraintName("FK_CropPest_CropPestDss")
                .OnDelete(DeleteBehavior.NoAction)
                .IsRequired();

            builder.HasMany<CropPestDssResult>(c => c.CropPestDssResults)
                .WithOne(cp => cp.CropPestDss)
                .HasForeignKey(c => c.CropPestDssId)
                .HasConstraintName("FK_CropPestDss_CropPestDssResult")
                .OnDelete(DeleteBehavior.Cascade)
                .IsRequired();
        }
    }
}