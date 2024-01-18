using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using H2020.IPMDecisions.UPR.Core.Models;
using H2020.IPMDecisions.UPR.Data.Core.Repositories;
using H2020.IPMDecisions.UPR.Data.Persistence.SqlQueries;
using Microsoft.EntityFrameworkCore;

namespace H2020.IPMDecisions.UPR.Data.Persistence.Repositories
{
    internal class DssResultRepository : IDssResultRepository
    {
        private ApplicationDbContext context;

        public DssResultRepository(ApplicationDbContext context)
        {
            this.context = context;
        }
        public async Task<List<DssResultDatabaseView>> GetAllDssResults(Guid userId)
        {
            return await this.context
               .DssResult
               .FromSqlRaw(RawSqlQueries.GetDssResults, userId)
               .ToListAsync();
        }
    }
}