using H2020.IPMDecisions.UPR.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace H2020.IPMDecisions.UPR.Data.Persistence.Configurations
{
    internal class FarmConfiguration : IEntityTypeConfiguration<Farm>
    {
        public void Configure(EntityTypeBuilder<Farm> builder)
        {
            builder.HasKey(f => f.Id);

            builder.Property(f => f.Id)
                .ValueGeneratedOnAdd();

            builder.HasIndex(f => f.Location)
                .IsUnique(false);

            builder.Property(f => f.Location)
                .HasColumnType("geometry (point)")
                .IsRequired();

            builder.HasMany<Field>(f => f.Fields)
                .WithOne(fi => fi.Farm)
                .HasForeignKey(fi => fi.FarmId)
                .HasConstraintName("FK_Field_Farm")
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne<WeatherHistorical>(f => f.WeatherHistorical)
                .WithMany(w => w.Farms)
                .HasForeignKey(f => f.WeatherHistoricalId)
                .IsRequired(false)
                .HasConstraintName("FK_Farm_WeatherHistorical")
                .OnDelete(DeleteBehavior.NoAction);

            builder.HasOne<WeatherForecast>(f => f.WeatherForecast)
                .WithMany(w => w.Farms)
                .HasForeignKey(f => f.WeatherForecastId)
                .IsRequired(false)
                .HasConstraintName("FK_Farm_WeatherForecast")
                .OnDelete(DeleteBehavior.NoAction);
        }
    }
}