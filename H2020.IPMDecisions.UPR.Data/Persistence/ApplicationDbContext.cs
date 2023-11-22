using H2020.IPMDecisions.UPR.Core;
using H2020.IPMDecisions.UPR.Core.Entities;
using H2020.IPMDecisions.UPR.Core.Models;
using H2020.IPMDecisions.UPR.Data.Persistence.Configurations;
using H2020.IPMDecisions.UPR.Data.Persistence.Extensions;
using Microsoft.EntityFrameworkCore;

namespace H2020.IPMDecisions.UPR.Data.Persistence
{
    public class ApplicationDbContext : DbContext, IApplicationDbContext
    {
        public DbSet<AdministrationVariable> AdministrationVariable { get; set; }
        public DbSet<CropPest> CropPest { get; set; }
        public DbSet<CropPestDss> CropPestDss { get; set; }
        public DbSet<DataSharingRequest> DataSharingRequest { get; set; }
        public DbSet<DssResultDatabaseView> DssResult { get; set; }
        public DbSet<DataSharingRequestStatus> DataSharingRequestStatus { get; set; }
        public DbSet<DisabledDss> DisabledDss { get; set; }
        public DbSet<EppoCode> EppoCode { get; set; }
        public DbSet<Farm> Farm { get; set; }
        public DbSet<Field> Field { get; set; }
        public DbSet<FieldDssResult> FieldDssResult { get; set; }
        public DbSet<FieldObservation> FieldObservation { get; set; }
        public DbSet<FieldCrop> FieldCrop { get; set; }
        public DbSet<FieldCropPest> FieldCropPest { get; set; }
        public DbSet<FieldCropPestDss> FieldCropPestDss { get; set; }
        public DbSet<FieldSprayApplication> FieldSprayApplication { get; set; }
        public DbSet<ForecastAlert> ForecastAlert { get; set; }
        public DbSet<ForecastResult> ForecastResult { get; set; }
        public DbSet<ObservationAlert> ObservationAlert { get; set; }
        public DbSet<ObservationResult> ObservationResult { get; set; }
        public DbSet<UserAddress> UserAddress { get; set; }
        public DbSet<UserFarm> UserFarm { get; set; }
        public DbSet<UserProfile> UserProfile { get; set; }
        public DbSet<UserFarmType> UserFarmType { get; set; }
        public DbSet<UserWidget> UserWidget { get; set; }
        public DbSet<WeatherForecast> WeatherForecast { get; set; }
        public DbSet<WeatherHistorical> WeatherHistorical { get; set; }
        public DbSet<Widget> Widget { get; set; }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasPostgresExtension("postgis");

            modelBuilder.ApplyConfiguration(new AdministrationVariableConfiguration());
            modelBuilder.ApplyConfiguration(new CropPestConfiguration());
            modelBuilder.ApplyConfiguration(new CropPestDssConfiguration());
            modelBuilder.ApplyConfiguration(new DataSharingRequestConfiguration());
            modelBuilder.ApplyConfiguration(new DataSharingRequestStatusConfiguration());
            modelBuilder.ApplyConfiguration(new DisabledDssConfiguration());
            modelBuilder.ApplyConfiguration(new EppoCodeConfiguration());
            modelBuilder.ApplyConfiguration(new FarmConfiguration());
            modelBuilder.ApplyConfiguration(new FieldCropPestConfiguration());
            modelBuilder.ApplyConfiguration(new FieldConfiguration());
            modelBuilder.ApplyConfiguration(new FieldDssResultConfiguration());
            modelBuilder.ApplyConfiguration(new FieldCropPestConfiguration());
            modelBuilder.ApplyConfiguration(new FieldCropPestDssConfiguration());
            modelBuilder.ApplyConfiguration(new FieldObservationConfiguration());
            modelBuilder.ApplyConfiguration(new FieldDssObservationConfiguration());
            modelBuilder.ApplyConfiguration(new FieldSprayApplicationConfiguration());
            modelBuilder.ApplyConfiguration(new ForecastAlertConfiguration());
            modelBuilder.ApplyConfiguration(new ForecastResultConfiguration());
            modelBuilder.ApplyConfiguration(new ObservationAlertConfiguration());
            modelBuilder.ApplyConfiguration(new ObservationResultConfiguration());
            modelBuilder.ApplyConfiguration(new UserFarmConfiguration());
            modelBuilder.ApplyConfiguration(new UserFarmTypeConfiguration());
            modelBuilder.ApplyConfiguration(new UserWidgetConfiguration());
            modelBuilder.ApplyConfiguration(new UserWeatherConfiguration());
            modelBuilder.ApplyConfiguration(new UserProfileConfiguration());
            modelBuilder.ApplyConfiguration(new UserAddressConfiguration());
            modelBuilder.ApplyConfiguration(new WeatherForecastConfiguration());
            modelBuilder.ApplyConfiguration(new WeatherHistoricalConfiguration());
            modelBuilder.ApplyConfiguration(new WidgetConfiguration());

            // Comment it out when adding new EF migration
            // modelBuilder.Ignore<DssResultDatabaseView>();
            modelBuilder.Seed();
        }
    }
}