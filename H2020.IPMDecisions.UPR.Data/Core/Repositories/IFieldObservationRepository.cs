using System;
using System.Threading.Tasks;
using H2020.IPMDecisions.UPR.Core.Dtos;
using H2020.IPMDecisions.UPR.Core.Entities;
using H2020.IPMDecisions.UPR.Core.Helpers;
using H2020.IPMDecisions.UPR.Core.ResourceParameters;

namespace H2020.IPMDecisions.UPR.Data.Core.Repositories
{
    public interface IFieldObservationRepository : IRepositoryBase<FieldObservation, FieldObservationResourceParameter>
    {
        Task<PagedList<FieldObservation>> FindAllAsync(FieldObservationResourceParameter resourceParameter, Guid? fieldId = null);
    }
}