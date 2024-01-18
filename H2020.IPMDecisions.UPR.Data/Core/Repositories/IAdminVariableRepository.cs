using System.Collections.Generic;
using System.Threading.Tasks;
using H2020.IPMDecisions.UPR.Core.Entities;
using H2020.IPMDecisions.UPR.Core.Enums;

namespace H2020.IPMDecisions.UPR.Data.Core.Repositories
{
    public interface IAdminVariableRepository
    {
        Task<AdministrationVariable> FindByIdAsync(AdminValuesEnum id);
        Task<List<AdministrationVariable>> FindAllAsync();
        void Update(AdministrationVariable entity);
    }
}