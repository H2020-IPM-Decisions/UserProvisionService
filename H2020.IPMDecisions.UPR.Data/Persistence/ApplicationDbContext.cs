using H2020.IPMDecisions.UPR.Core;
using H2020.IPMDecisions.UPR.Core.Entities;
using H2020.IPMDecisions.UPR.Data.Persistence.Configurations;
using H2020.IPMDecisions.UPR.Data.Persistence.Extensions;
using Microsoft.EntityFrameworkCore;

namespace H2020.IPMDecisions.UPR.Data.Persistence
{
    public class ApplicationDbContext : DbContext, IApplicationDbContext
    {
        public DbSet<CropPest> CropPest { get; set; }
        public DbSet<CropPestDss> CropPestDss { get; set; }
        public DbSet<DataSharingRequest> DataSharingRequest { get; set; }
        public DbSet<DataSharingRequestStatus> DataSharingRequestStatus { get; set; }
        public DbSet<Farm> Farm { get; set; }
        public DbSet<FarmWeatherDataSource> FarmWeatherDataSource { get; set; }
        public DbSet<FarmWeatherStation> FarmWeatherStation { get; set; }
        public DbSet<Field> Field { get; set; }
        public DbSet<FieldObservation> FieldObservation { get; set; }
        public DbSet<FieldCropPest> FieldCropPest { get; set; }
        public DbSet<FieldCropPestDss> FieldCropPestDss { get; set; }
        public DbSet<FieldSprayApplication> FieldSprayApplication { get; set; }
        public DbSet<FieldWeatherDataSource> FieldWeatherDataSource { get; set; }
        public DbSet<FieldWeatherStation> FieldWeatherStation { get; set; }
        public DbSet<ForecastAlert> ForecastAlert { get; set; }
        public DbSet<ForecastResult> ForecastResult { get; set; }
        public DbSet<ObservationAlert> ObservationAlert { get; set; }
        public DbSet<ObservationResult> ObservationResult { get; set; }
        public DbSet<UserAddress> UserAddress { get; set; }
        public DbSet<UserFarm> UserFarm { get; set; }
        public DbSet<UserProfile> UserProfile { get; set; }
        public DbSet<UserFarmType> UserFarmType { get; set; }
        public DbSet<UserWidget> UserWidget { get; set; }
        public DbSet<WeatherDataSource> WeatherDataSource { get; set; }
        public DbSet<WeatherStation> WeatherStation { get; set; }
        public DbSet<Widget> Widget { get; set; }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasPostgresExtension("postgis");

            modelBuilder.ApplyConfiguration(new CropPestConfiguration());
            modelBuilder.ApplyConfiguration(new CropPestDssConfiguration());
            modelBuilder.ApplyConfiguration(new FieldCropPestConfiguration());
            modelBuilder.ApplyConfiguration(new DataSharingRequestConfiguration());
            modelBuilder.ApplyConfiguration(new DataSharingRequestStatusConfiguration());
            modelBuilder.ApplyConfiguration(new FarmConfiguration());
            modelBuilder.ApplyConfiguration(new FarmWeatherDataSourceConfiguration());
            modelBuilder.ApplyConfiguration(new FarmWeatherStationConfiguration());
            modelBuilder.ApplyConfiguration(new FieldConfiguration());
            modelBuilder.ApplyConfiguration(new FieldCropPestConfiguration());
            modelBuilder.ApplyConfiguration(new FieldCropPestDssConfiguration());
            modelBuilder.ApplyConfiguration(new FieldObservationConfiguration());
            modelBuilder.ApplyConfiguration(new FieldSprayApplicationConfiguration());
            modelBuilder.ApplyConfiguration(new FieldWeatherDataSourceConfiguration());
            modelBuilder.ApplyConfiguration(new FieldWeatherStationConfiguration());
            modelBuilder.ApplyConfiguration(new ForecastAlertConfiguration());
            modelBuilder.ApplyConfiguration(new ForecastResultConfiguration());
            modelBuilder.ApplyConfiguration(new ObservationAlertConfiguration());
            modelBuilder.ApplyConfiguration(new ObservationResultConfiguration());
            modelBuilder.ApplyConfiguration(new UserFarmConfiguration());
            modelBuilder.ApplyConfiguration(new UserFarmTypeConfiguration());
            modelBuilder.ApplyConfiguration(new UserWidgetConfiguration());
            modelBuilder.ApplyConfiguration(new UserProfileConfiguration());
            modelBuilder.ApplyConfiguration(new UserAddressConfiguration());
            modelBuilder.ApplyConfiguration(new WidgetConfiguration());

            modelBuilder.Seed();
        }
    }
}