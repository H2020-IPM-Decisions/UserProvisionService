using System.Collections.Generic;
using System.Threading.Tasks;
using H2020.IPMDecisions.UPR.Core.Entities;

namespace H2020.IPMDecisions.UPR.Data.Core.Repositories
{
    public interface IEppoCodeRepository
    {
        Task<List<EppoCode>> GetEppoCodesAsync();
    }
}