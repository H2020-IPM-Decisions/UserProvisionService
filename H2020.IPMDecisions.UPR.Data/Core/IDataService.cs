using System;
using System.Threading.Tasks;
using H2020.IPMDecisions.UPR.Data.Core.Repositories;

namespace H2020.IPMDecisions.UPR.Data.Core
{
    public interface IDataService : IDisposable
    {
        Task CompleteAsync();
        ICropPestRepository CropPests { get; }
        IDataShareRequestRepository DataShareRequests { get; }
        IDataSharingRequestStatusRepository DataSharingRequestStatuses { get; }
        IFarmRepository Farms { get; }
        IFieldRepository Fields { get; }
        IFieldObservationRepository FieldObservations { get; }
        IFieldCropPestRepository FieldCropPests { get; }
        IUserFarmsRepository UserFarms { get; }
        IUserProfileRepository UserProfiles { get; }
    }
}