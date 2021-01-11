using System;
using System.Threading.Tasks;
using H2020.IPMDecisions.UPR.Core.Entities;
using H2020.IPMDecisions.UPR.Core.Helpers;
using H2020.IPMDecisions.UPR.Core.ResourceParameters;

namespace H2020.IPMDecisions.UPR.Data.Core.Repositories
{
    public interface IFieldSprayApplicationRepository : IRepositoryBaseWithResourceParameter<FieldSprayApplication, FieldSprayResourceParameter>
    {
        Task<PagedList<FieldSprayApplication>> FindAllAsync(FieldSprayResourceParameter resourceParameter, Guid? fieldId = null);
    }
}