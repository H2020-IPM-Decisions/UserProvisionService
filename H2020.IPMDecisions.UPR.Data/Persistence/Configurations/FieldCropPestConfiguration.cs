using H2020.IPMDecisions.UPR.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace H2020.IPMDecisions.UPR.Data.Persistence.Configurations
{
    internal class FieldCropPestConfiguration : IEntityTypeConfiguration<FieldCropPest>
    {
        public void Configure(EntityTypeBuilder<FieldCropPest> builder)
        {
            builder.HasKey(cp => cp.Id);

            builder.Property(cp => cp.Id)
                .ValueGeneratedOnAdd();

            builder.HasIndex(cp =>
                new
                {
                    cp.FieldCropId,
                    cp.CropPestId
                })
                .IsUnique();

            builder.HasOne<CropPest>(cp => cp.CropPest)
                .WithMany(c => c.FieldCropPests)
                .HasForeignKey(cp => cp.CropPestId)
                .HasConstraintName("FK_CropPest_Crop")
                .OnDelete(DeleteBehavior.Cascade)
                .IsRequired();

            builder.HasMany<FieldObservation>(f => f.FieldObservations)
                .WithOne(fo => fo.FieldCropPest)
                .HasForeignKey(f => f.FieldCropPestId)
                .HasConstraintName("FK_Observation_FieldCropPest")
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany<FieldSprayApplication>(f => f.FieldSprayApplications)
                .WithOne(s => s.FieldCropPest)
                .HasForeignKey(f => f.FieldCropPestId)
                .HasConstraintName("FK_Spray_FieldCropPest")
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}