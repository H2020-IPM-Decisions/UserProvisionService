using System.Collections.Generic;
using System.Linq;
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

        public void Create(EppoCode entity)
        {
            this.context.Add(entity);
        }

        public async Task<List<EppoCode>> GetEppoCodesAsync(string eppoCodeType = "")
        {
            var collection = this.context.EppoCode as IQueryable<EppoCode>;

            if (!string.IsNullOrEmpty(eppoCodeType))
                collection = collection.Where(e => e.Type.ToLower() == eppoCodeType);

            return await collection.ToListAsync();
        }

        public async Task<List<string>> GetEppoCodeTypesAsync()
        {
            return await this.context.EppoCode.Select(e => e.Type).ToListAsync();
        }

        public void Update(EppoCode entity)
        {
            this.context.Update(entity);
        }
    }
}