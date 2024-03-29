using System.Collections.Generic;
using System.Threading.Tasks;
using H2020.IPMDecisions.UPR.Core.Entities;

namespace H2020.IPMDecisions.UPR.Data.Core.Repositories
{
    public interface IEppoCodeRepository
    {
        void Create(EppoCode entity);
        void Update(EppoCode entity);
        Task<List<EppoCode>> GetEppoCodesAsync(string eppoCodeType = "");
        Task<List<string>> GetEppoCodeTypesAsync();
    }
}