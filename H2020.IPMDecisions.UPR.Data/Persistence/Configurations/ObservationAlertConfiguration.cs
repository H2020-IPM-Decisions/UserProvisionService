using H2020.IPMDecisions.UPR.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace H2020.IPMDecisions.UPR.Data.Persistence.Configurations
{
    internal class ObservationAlertConfiguration : IEntityTypeConfiguration<ObservationAlert>
    {
        public void Configure(EntityTypeBuilder<ObservationAlert> builder)
        {
            builder.HasKey(o => o.Id);

            builder.Property(o => o.Id)
                .ValueGeneratedOnAdd();

            builder.HasIndex(o =>
                new
                {
                    o.WeatherStationId,
                    o.CropPestDssCombinationId,
                    o.FieldObservationId
                })
           .IsUnique();

            builder.HasOne<CropPestDssCombination>(o => o.CropPestDssCombination)
                .WithMany(cp => cp.ObservationAlerts)
                .HasForeignKey(o => o.CropPestDssCombinationId)
                .HasConstraintName("FK_ObservationAlert_CropPestDssCombination_Id")
                .OnDelete(DeleteBehavior.NoAction)
                .IsRequired();

            builder.HasOne<FieldObservation>(f => f.FieldObservation)
                .WithMany(fo => fo.ObservationAlerts)
                .HasForeignKey(f => f.FieldObservationId)
                .OnDelete(DeleteBehavior.NoAction)
                .IsRequired();
        }
    }
}