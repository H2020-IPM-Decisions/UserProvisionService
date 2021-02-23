using H2020.IPMDecisions.UPR.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace H2020.IPMDecisions.UPR.Data.Persistence.Configurations
{
    internal class FieldWeatherDataSourceConfiguration : IEntityTypeConfiguration<FieldWeatherDataSource>
    {
        public void Configure(EntityTypeBuilder<FieldWeatherDataSource> builder)
        {
            builder.HasKey(fw =>
                new
                {
                    fw.FieldId,
                    fw.WeatherDataSourceId
                });

            builder.HasOne<Field>(fw => fw.Field)
                .WithMany(f => f.FieldWeatherDataSources)
                .HasForeignKey(fw => fw.FieldId)
                .HasConstraintName("FK_FieldWeatherDataSource_Field")
                .OnDelete(DeleteBehavior.Cascade)
                .IsRequired();

            builder.HasOne<WeatherDataSource>(fw => fw.WeatherDataSource)
               .WithMany(w => w.FieldWeatherDataSources)
               .HasForeignKey(fw => fw.WeatherDataSourceId)
               .HasConstraintName("FK_FieldWeatherDataSource_WeatherDataSource")
               .OnDelete(DeleteBehavior.Cascade)
               .IsRequired();
        }
    }
}