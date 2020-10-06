using System;
using System.Threading.Tasks;
using H2020.IPMDecisions.UPR.Core.Entities;
using H2020.IPMDecisions.UPR.Core.Helpers;
using H2020.IPMDecisions.UPR.Core.ResourceParameters;

namespace H2020.IPMDecisions.UPR.Data.Core.Repositories
{
    public interface IFieldCropPestRepository : IRepositoryBase<FieldCropPest, FieldCropPestResourceParameter>
    {
        Task<PagedList<FieldCropPest>> FindAllAsync(FieldCropPestResourceParameter resourceParameter, Guid fieldId);
        Task<PagedList<FieldCropPest>> FindAllAsync(FieldCropPestResourceParameter resourceParameter, Guid fieldId, bool includeAssociatedData);
    }
}