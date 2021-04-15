using H2020.IPMDecisions.UPR.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Newtonsoft.Json;

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

            builder.HasMany<FieldDssResult>(cpd => cpd.FieldDssResults)
                .WithOne(f => f.FieldCropPestDss)
                .HasForeignKey(f => f.FieldCropPestDssId)
                .OnDelete(DeleteBehavior.Cascade)
                .IsRequired();

            builder
                .Property(cpd => cpd.DssParameters)
                .HasColumnType("jsonb");

            builder.HasMany<FieldDssObservation>(f => f.FieldDssObservations)
                .WithOne(fo => fo.FieldCropPestDss)
                .HasForeignKey(f => f.FieldCropPestDssId)
                .HasConstraintName("FK_Observation_FieldCropPestDss")
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}