using System;
using System.Threading.Tasks;
using H2020.IPMDecisions.UPR.Data.Core.Repositories;

namespace H2020.IPMDecisions.UPR.Data.Core
{
    public interface IDataService : IDisposable
    {
        Task CompleteAsync();
        ICropPestRepository CropPests { get; }
        ICropPestDssRepository CropPestDsses { get; }
        IDssResultRepository DssResult { get; }
        IDataShareRequestRepository DataShareRequests { get; }
        IDataSharingRequestStatusRepository DataSharingRequestStatuses { get; }
        IEppoCodeRepository EppoCodes { get; }
        IFarmRepository Farms { get; }
        IFieldRepository Fields { get; }
        IFieldObservationRepository FieldObservations { get; }
        IFieldCropPestRepository FieldCropPests { get; }
        IFieldCropPestDssRepository FieldCropPestDsses { get; }
        IFieldSprayApplicationRepository FieldSprayApplication { get; }
        IUserFarmsRepository UserFarms { get; }
        IUserProfileRepository UserProfiles { get; }
        IUserWidgetRepository UserWidgets { get; }
        IWeatherHistoricalRepository WeatherHistoricals { get; }
        IWeatherForecastRepository WeatherForecasts { get; }
    }
}