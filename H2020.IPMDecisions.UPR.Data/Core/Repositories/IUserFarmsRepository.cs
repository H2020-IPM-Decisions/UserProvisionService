using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using H2020.IPMDecisions.UPR.Core.Entities;

namespace H2020.IPMDecisions.UPR.Data.Core.Repositories
{
    public interface IUserFarmsRepository
    {
        void Delete(UserFarm entity);
        void DeleteRange(List<UserFarm> entities);
        Task<List<UserFarm>> GetReportDataAsync();        
        Task<IEnumerable<UserFarm>> FindAllAsync(Expression<Func<UserFarm, bool>> expression);
    }
}