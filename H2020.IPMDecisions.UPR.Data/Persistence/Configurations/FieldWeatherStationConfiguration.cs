using H2020.IPMDecisions.UPR.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace H2020.IPMDecisions.UPR.Data.Persistence.Configurations
{
    internal class FieldWeatherStationConfiguration : IEntityTypeConfiguration<FieldWeatherStation>
    {
        public void Configure(EntityTypeBuilder<FieldWeatherStation> builder)
        {
            builder.HasKey(fw =>
               new
               {
                   fw.FieldId,
                   fw.WeatherStationId
               });

            builder.HasOne<Field>(fw => fw.Field)
                .WithMany(f => f.FieldWeatherStations)
                .HasForeignKey(fw => fw.FieldId)
                .HasConstraintName("FK_FieldWeatherStation_Field")
                .OnDelete(DeleteBehavior.Cascade)
                .IsRequired();

            builder.HasOne<WeatherStation>(fw => fw.WeatherStation)
               .WithMany(w => w.FieldWeatherStations)
               .HasForeignKey(fw => fw.WeatherStationId)
               .HasConstraintName("FK_FieldWeatherStation_WeatherStation")
               .OnDelete(DeleteBehavior.Cascade)
               .IsRequired();
        }
    }
}