using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using H2020.IPMDecisions.UPR.Core.Entities;
using H2020.IPMDecisions.UPR.Data.Core.Repositories;
using Microsoft.EntityFrameworkCore;

namespace H2020.IPMDecisions.UPR.Data.Persistence.Repositories
{
    internal class CropPestDssRepository : ICropPestDssRepository
    {
        private ApplicationDbContext context;

        public CropPestDssRepository(ApplicationDbContext context)
        {
            this.context = context;
        }

        public void Create(CropPestDss entity)
        {
            this.context.Add(entity);
        }

        public async Task<CropPestDss> FindByConditionAsync(Expression<Func<CropPestDss, bool>> expression)
        {
            return await this.context
                .CropPestDss
                .Where(expression)
                .FirstOrDefaultAsync();
        }
    }
}