using System;
using System.Threading.Tasks;
using H2020.IPMDecisions.UPR.Core.Entities;
using H2020.IPMDecisions.UPR.Core.Enums;
using H2020.IPMDecisions.UPR.Core.Helpers;
using H2020.IPMDecisions.UPR.Core.ResourceParameters;

namespace H2020.IPMDecisions.UPR.Data.Core.Repositories
{
    public interface IDataShareRequestRepository : IRepositoryBaseWithResourceParameter<DataSharingRequest, DataShareResourceParameter>
    {
        Task Create(Guid requesterId, Guid requesteeId, RequestStatusEnum requestStatus);
        Task<PagedList<DataSharingRequest>> FindAllAsync(Guid userId, DataShareResourceParameter resourceParameter);
    }
}