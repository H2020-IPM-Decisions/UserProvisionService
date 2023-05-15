using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using H2020.IPMDecisions.UPR.Core.Entities;

namespace H2020.IPMDecisions.UPR.Data.Core.Repositories
{
    public interface IDisabledDssRepository
    {
        Task Create(List<DisabledDss> entities);
        Task<List<DisabledDss>> FindAllByConditionAsync(Expression<Func<DisabledDss, bool>> expression);
        Task<List<DisabledDss>> GetAllAsync();
        Task Delete(List<Guid> ids);
    }
}