using System.Collections.Generic;
using System.Threading.Tasks;
using H2020.IPMDecisions.UPR.Core.Entities;
using H2020.IPMDecisions.UPR.Data.Core.Repositories;
using Microsoft.EntityFrameworkCore;

namespace H2020.IPMDecisions.UPR.Data.Persistence.Repositories
{
    internal class EppoCodeRepository : IEppoCodeRepository
    {
        private ApplicationDbContext context;

        public EppoCodeRepository(ApplicationDbContext context)
        {
            this.context = context
                ?? throw new System.ArgumentNullException(nameof(context));
        }

        public async Task<List<EppoCode>> GetEppoCodesAsync()
        {
            return await this.context.EppoCode.ToListAsync();
        }
    }
}