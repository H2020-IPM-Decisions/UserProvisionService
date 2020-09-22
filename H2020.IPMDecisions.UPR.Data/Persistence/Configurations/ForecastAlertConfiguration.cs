using H2020.IPMDecisions.UPR.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace H2020.IPMDecisions.UPR.Data.Persistence.Configurations
{
    internal class ForecastAlertConfiguration : IEntityTypeConfiguration<ForecastAlert>
    {
        public void Configure(EntityTypeBuilder<ForecastAlert> builder)
        {
            builder.HasKey(f => f.Id);

            builder.Property(f => f.Id)
                .ValueGeneratedOnAdd();

            builder.HasIndex(f =>
                new
                {
                    f.WeatherStationId,
                    f.CropPestDssCombinationId
                })
                .IsUnique();

            builder.HasOne<CropPestDssCombination>(f => f.CropPestDssCombination)
                .WithMany(cp => cp.ForecastAlerts)
                .HasForeignKey(f => f.CropPestDssCombinationId)
                .HasConstraintName("FK_ForecastAlert_CropPestDssCombination_Id")
                .OnDelete(DeleteBehavior.NoAction)
                .IsRequired();
        }
    }
}