using H2020.IPMDecisions.UPR.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace H2020.IPMDecisions.UPR.Data.Persistence.Configurations
{
    internal class WeatherDataSourceConfiguration : IEntityTypeConfiguration<WeatherDataSource>
    {
        public void Configure(EntityTypeBuilder<WeatherDataSource> builder)
        {
            builder.HasKey(f => f.Id);

            builder.Property(f => f.Id)
                .ValueGeneratedOnAdd();

            builder.Property(f => f.Credentials)
                .HasColumnType("jsonb");

            builder.HasOne<Farm>(fw => fw.Farm)
                .WithMany(f => f.FarmWeatherDataSources)
                .HasForeignKey(fw => fw.FarmId)
                .HasConstraintName("FK_FarmWeatherDataSource_Farm")
                .OnDelete(DeleteBehavior.Cascade)
                .IsRequired();
        }
    }
}