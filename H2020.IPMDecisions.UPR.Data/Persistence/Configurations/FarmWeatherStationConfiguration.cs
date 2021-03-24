// using H2020.IPMDecisions.UPR.Core.Entities;
// using Microsoft.EntityFrameworkCore;
// using Microsoft.EntityFrameworkCore.Metadata.Builders;

// namespace H2020.IPMDecisions.UPR.Data.Persistence.Configurations
// {
//     internal class FarmWeatherStationConfiguration : IEntityTypeConfiguration<FarmWeatherStation>
//     {
//         public void Configure(EntityTypeBuilder<FarmWeatherStation> builder)
//         {
//             builder.HasKey(fw =>
//                 new
//                 {
//                     fw.FarmId,
//                     fw.WeatherStationId
//                 });

//             builder.HasOne<Farm>(fw => fw.Farm)
//                 .WithMany(f => f.FarmWeatherStations)
//                 .HasForeignKey(fw => fw.FarmId)
//                 .HasConstraintName("FK_FarmWeatherStation_Farm")
//                 .OnDelete(DeleteBehavior.Cascade)
//                 .IsRequired();

//             builder.HasOne<WeatherStation>(fw => fw.WeatherStation)
//                .WithMany(w => w.FarmWeatherStations)
//                .HasForeignKey(fw => fw.WeatherStationId)
//                .HasConstraintName("FK_FarmWeatherStation_WeatherStation")
//                .OnDelete(DeleteBehavior.Cascade)
//                .IsRequired();
//         }
//     }
// }