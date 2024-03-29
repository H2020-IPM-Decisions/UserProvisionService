using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using H2020.IPMDecisions.UPR.Core.Entities;
using H2020.IPMDecisions.UPR.Core.Helpers;
using H2020.IPMDecisions.UPR.Core.ResourceParameters;

namespace H2020.IPMDecisions.UPR.Data.Core.Repositories
{
    public interface IFarmRepository : IRepositoryBaseWithResourceParameter<Farm, FarmResourceParameter>
    {
        Task<PagedList<Farm>> FindAllAsync(FarmResourceParameter resourceParameter, Guid userId);
        Task<PagedList<Farm>> FindAllAsync(FarmResourceParameter resourceParameter, Guid userId, bool includeAssociatedData);
        Task<Farm> FindByIdAsync(Guid id, bool includeAssociatedData);
        Task<IEnumerable<Farm>> FindAllByConditionAsync(Expression<Func<Farm, bool>> expression);
    }
}