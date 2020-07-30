using System;
using System.Threading.Tasks;
using H2020.IPMDecisions.UPR.Core.Entities;
using H2020.IPMDecisions.UPR.Core.Helpers;
using H2020.IPMDecisions.UPR.Core.ResourceParameters;

namespace H2020.IPMDecisions.UPR.Data.Core.Repositories
{
    public interface IFieldRepository : IRepositoryBase<Field, FieldResourceParameter>
    {
        Task<PagedList<Field>> FindAllAsync(FieldResourceParameter resourceParameter, Guid farmId);
        Task<PagedList<Field>> FindAllAsync(FieldResourceParameter resourceParameter, Guid farmId, bool includeAssociatedData);
        Task<Field> FindByIdAsync(Guid id, bool includeAssociatedData);
    }
}