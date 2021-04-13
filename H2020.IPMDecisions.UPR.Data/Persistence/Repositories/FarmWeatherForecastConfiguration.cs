using H2020.IPMDecisions.UPR.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace H2020.IPMDecisions.UPR.Data.Persistence.Configurations
{
    internal class FarmWeatherForecastConfiguration : IEntityTypeConfiguration<FarmWeatherForecast>
    {
        public void Configure(Microsoft.EntityFrameworkCore.Metadata.Builders.EntityTypeBuilder<FarmWeatherForecast> builder)
        {
            builder
                .HasKey(fw => new { fw.FarmId, fw.WeatherForecastId });

            builder
                .HasOne(fw => fw.Farm)
                .WithMany(f => f.FarmWeatherForecast)
                .HasForeignKey(fw => fw.FarmId);

            builder
               .HasOne(fw => fw.WeatherForecast)
               .WithMany(w => w.FarmWeatherForecast)
               .HasForeignKey(fw => fw.WeatherForecastId);
        }
    }
}