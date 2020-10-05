using H2020.IPMDecisions.UPR.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace H2020.IPMDecisions.UPR.Data.Persistence.Configurations
{
    internal class FieldCropPestDssConfiguration : IEntityTypeConfiguration<FieldCropPestDss>
    {
        public void Configure(EntityTypeBuilder<FieldCropPestDss> builder)
        {
            builder.HasKey(cpd => cpd.Id);

            builder.Property(cpd => cpd.Id)
                .ValueGeneratedOnAdd();

            builder.HasIndex(cpd =>
                new
                {
                    cpd.FieldCropPestId,
                    cpd.CropPestDssId
                })
                .IsUnique();

            builder.HasOne<CropPestDss>(cpd => cpd.CropPestDss)
                .WithMany(c => c.FieldCropPestDsses)
                .HasForeignKey(cp => cp.CropPestDssId)
                .HasConstraintName("FK_FieldCropPestDss_CropPestDss")
                .OnDelete(DeleteBehavior.Cascade)
                .IsRequired();
        }
    }
}