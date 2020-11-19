using H2020.IPMDecisions.UPR.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace H2020.IPMDecisions.UPR.Data.Persistence.Configurations
{
    internal class FarmWeatherDataSourceConfiguration : IEntityTypeConfiguration<FarmWeatherDataSource>
    {
        public void Configure(EntityTypeBuilder<FarmWeatherDataSource> builder)
        {
            builder.HasKey(fw =>
                new
                {
                    fw.FarmId,
                    fw.WeatherForecastServiceId
                });

            builder.HasOne<Farm>(fw => fw.Farm)
                .WithMany(f => f.FarmWeatherForecastServices)
                .HasForeignKey(fw => fw.FarmId)
                .HasConstraintName("FK_FarmWeatherDataSource_Farm")
                .OnDelete(DeleteBehavior.Cascade)
                .IsRequired();

            builder.HasOne<WeatherDataSource>(fw => fw.WeatherDataSource)
               .WithMany(w => w.FarmWeatherForecastServices)
               .HasForeignKey(fw => fw.WeatherForecastServiceId)
               .HasConstraintName("FK_FarmWeatherDataSource_WeatherDataSource")
               .OnDelete(DeleteBehavior.Cascade)
               .IsRequired();
        }
    }
}