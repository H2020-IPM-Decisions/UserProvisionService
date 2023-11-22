using H2020.IPMDecisions.UPR.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace H2020.IPMDecisions.UPR.Data.Persistence.Configurations
{
    internal class UserWeatherConfiguration : IEntityTypeConfiguration<UserWeather>
    {
        public void Configure(EntityTypeBuilder<UserWeather> builder)
        {
            builder.HasKey(uw => uw.Id);

            builder.Property(f => f.Id)
                .ValueGeneratedOnAdd();

            builder.Property(uw => uw.WeatherId)
               .IsRequired();
            
            builder.Property(uw => uw.WeatherStationReference)
               .IsRequired();

            builder.Property(uw => uw.UserName)
                .IsRequired();

            builder.Property(uw => uw.Password)
                .IsRequired();

            builder.Property(uw => uw.WeatherStationId)
               .IsRequired();

            builder.HasOne<UserProfile>(uw => uw.UserProfile)
                .WithMany(u => u.UserWeathers)
                .HasForeignKey(uw => uw.UserProfileId)
                .HasConstraintName("FK_UserWeather_UserProfile")
                .OnDelete(DeleteBehavior.Cascade)
                .IsRequired();
        }
    }
}