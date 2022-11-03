using System;
using System.Linq.Expressions;
using System.Threading.Tasks;
using H2020.IPMDecisions.UPR.Core.Entities;

namespace H2020.IPMDecisions.UPR.Data.Core.Repositories
{
    public interface IDataSharingRequestStatusRepository
    {
        Task<DataSharingRequestStatus> FindByCondition(Expression<Func<DataSharingRequestStatus, bool>> expression);
    }
}