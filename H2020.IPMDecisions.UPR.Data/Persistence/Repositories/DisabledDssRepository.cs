using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using H2020.IPMDecisions.UPR.Core.Entities;
using H2020.IPMDecisions.UPR.Data.Core.Repositories;
using Microsoft.EntityFrameworkCore;

namespace H2020.IPMDecisions.UPR.Data.Persistence.Repositories
{
    internal class DisabledDssRepository : IDisabledDssRepository
    {
        private ApplicationDbContext context;

        public DisabledDssRepository(ApplicationDbContext context)
        {
            this.context = context;
        }
        public async Task Delete(List<Guid> ids)
        {
            var entities = await FindAllByConditionAsync(d => ids.Contains(d.Id));
            if (entities == null) return;
            this.context.DisabledDss.RemoveRange(entities);
        }

        public async Task<List<DisabledDss>> FindAllByConditionAsync(Expression<Func<DisabledDss, bool>> expression)
        {
            return await this.context
                .DisabledDss
                .Where(expression)
                .ToListAsync<DisabledDss>();
        }

        public async Task<List<DisabledDss>> GetAllAsync()
        {
            return await this.context
                .DisabledDss
                .ToListAsync();
        }

        public async Task Create(List<DisabledDss> entities)
        {
            await this.context
                .DisabledDss
                .AddRangeAsync(entities);
        }
    }
}