using H2020.IPMDecisions.UPR.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace H2020.IPMDecisions.UPR.Core
{
    public interface IApplicationDbContext
    {
        DbSet<CropPest> CropPest { get; set; }
        DbSet<CropPestDss> CropPestDss { get; set; }
        DbSet<DataSharingRequest> DataSharingRequest { get; set; }
        DbSet<DataSharingRequestStatus> DataSharingRequestStatus { get; set; }
        DbSet<Farm> Farm { get; set; }
        DbSet<FarmWeatherDataSource> FarmWeatherDataSource { get; set; }
        DbSet<FarmWeatherStation> FarmWeatherStation { get; set; }
        DbSet<Field> Field { get; set; }
        DbSet<FieldObservation> FieldObservation { get; set; }
        DbSet<FieldCropPest> FieldCropPest { get; set; }
        DbSet<FieldCropPestDss> FieldCropPestDss { get; set; }
        DbSet<ForecastAlert> ForecastAlert { get; set; }
        DbSet<ForecastResult> ForecastResult { get; set; }
        DbSet<ObservationAlert> ObservationAlert { get; set; }
        DbSet<ObservationResult> ObservationResult { get; set; }
        DbSet<UserAddress> UserAddress { get; set; }
        DbSet<UserFarm> UserFarm { get; set; }
        DbSet<UserFarmType> UserFarmType { get; set; }
        DbSet<UserProfile> UserProfile { get; set; }
        DbSet<WeatherDataSource> WeatherDataSource { get; set; }
        DbSet<WeatherStation> WeatherStation { get; set; }
    }
}