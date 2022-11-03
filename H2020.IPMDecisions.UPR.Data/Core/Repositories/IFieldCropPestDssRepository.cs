using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using H2020.IPMDecisions.UPR.Core.Entities;
using H2020.IPMDecisions.UPR.Core.Helpers;
using H2020.IPMDecisions.UPR.Core.ResourceParameters;

namespace H2020.IPMDecisions.UPR.Data.Core.Repositories
{
    public interface IFieldCropPestDssRepository : IRepositoryBaseWithResourceParameter<FieldCropPestDss, FieldCropPestDssResourceParameter>
    {
        Task<IEnumerable<FieldCropPestDss>> FindAllAsync(Expression<Func<FieldCropPestDss, bool>> expression);
        Task<IEnumerable<FieldCropPestDss>> FindAllAsync(Expression<Func<FieldCropPestDss, bool>> expression, int pageNumber, int pageSize);
        Task<PagedList<FieldCropPestDss>> FindAllAsync(FieldCropPestDssResourceParameter resourceParameter, bool includeAssociatedData);
        Task DeleteDssResultsByCondition(Expression<Func<FieldDssResult, bool>> expression);
        void AddDssResult(FieldCropPestDss entity, FieldDssResult dssResult);
        int GetCount(Expression<Func<FieldCropPestDss, bool>> expression);
        Task<bool> HasAny(Expression<Func<FieldCropPestDss, bool>> expression);
    }
}