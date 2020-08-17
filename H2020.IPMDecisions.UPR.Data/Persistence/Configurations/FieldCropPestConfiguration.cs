using H2020.IPMDecisions.UPR.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace H2020.IPMDecisions.UPR.Data.Persistence.Configurations
{
    internal class FieldCropPestConfiguration : IEntityTypeConfiguration<FieldCropPest>
    {
        public void Configure(EntityTypeBuilder<FieldCropPest> builder)
        {
            builder.HasKey(cp =>
                new
                {
                    cp.FieldId,
                    cp.CropPestId
                });

            builder.HasOne<CropPest>(cp => cp.CropPest)
                .WithMany(c => c.FieldCropPests)
                .HasForeignKey(cp => cp.CropPestId)
                .HasConstraintName("FK_CropPest_Crop")
                .OnDelete(DeleteBehavior.Cascade)
                .IsRequired();

            builder.HasOne<Field>(cp => cp.Field)
                .WithMany(f => f.FieldCropPests)
                .HasForeignKey(cp => cp.FieldId)
                .HasConstraintName("FK_CropPest_Field")
                .OnDelete(DeleteBehavior.Cascade)
                .IsRequired();
        }
    }
}