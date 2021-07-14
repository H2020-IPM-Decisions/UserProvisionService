using H2020.IPMDecisions.UPR.Core.Entities;
using H2020.IPMDecisions.UPR.Core.Models;
using Microsoft.EntityFrameworkCore;

namespace H2020.IPMDecisions.UPR.Core
{
    public interface IApplicationDbContext
    {
        DbSet<AdministrationVariable> AdministrationVariable { get; set; }
        DbSet<CropPest> CropPest { get; set; }
        DbSet<CropPestDss> CropPestDss { get; set; }
        DbSet<DataSharingRequest> DataSharingRequest { get; set; }
        DbSet<DssResultDatabaseView> DssResult { get; set; }
        DbSet<DataSharingRequestStatus> DataSharingRequestStatus { get; set; }
        DbSet<EppoCode> EppoCode { get; set; }
        DbSet<Farm> Farm { get; set; }
        DbSet<Field> Field { get; set; }
        DbSet<FieldDssResult> FieldDssResult { get; set; }
        DbSet<FieldObservation> FieldObservation { get; set; }
        DbSet<FieldCrop> FieldCrop { get; set; }
        DbSet<FieldCropPest> FieldCropPest { get; set; }
        DbSet<FieldCropPestDss> FieldCropPestDss { get; set; }
        DbSet<FieldSprayApplication> FieldSprayApplication { get; set; }
        DbSet<ForecastAlert> ForecastAlert { get; set; }
        DbSet<ForecastResult> ForecastResult { get; set; }
        DbSet<ObservationAlert> ObservationAlert { get; set; }
        DbSet<ObservationResult> ObservationResult { get; set; }
        DbSet<UserAddress> UserAddress { get; set; }
        DbSet<UserFarm> UserFarm { get; set; }
        DbSet<UserFarmType> UserFarmType { get; set; }
        DbSet<UserProfile> UserProfile { get; set; }
        DbSet<UserWidget> UserWidget { get; set; }
        DbSet<WeatherForecast> WeatherForecast { get; set; }
        DbSet<WeatherHistorical> WeatherHistorical { get; set; }
        DbSet<Widget> Widget { get; set; }
    }
}