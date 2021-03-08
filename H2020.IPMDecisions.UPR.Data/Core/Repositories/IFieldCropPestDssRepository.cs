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
        Task<List<FieldCropPestDss>> FindAllAsync(Expression<Func<FieldCropPestDss, bool>> expression);
        Task<PagedList<FieldCropPestDss>> FindAllAsync(FieldCropPestDssResourceParameter resourceParameter, bool includeAssociatedData);
        void AddDssResult(FieldCropPestDss entity, FieldDssResult dssResult);
    }
}