using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using H2020.IPMDecisions.UPR.Core.Models;

namespace H2020.IPMDecisions.UPR.Data.Core.Repositories
{
    public interface IDssResultRepository
    {
        Task<List<DssResultDatabaseView>> GetAllDssResults(Guid userId);
    }
}