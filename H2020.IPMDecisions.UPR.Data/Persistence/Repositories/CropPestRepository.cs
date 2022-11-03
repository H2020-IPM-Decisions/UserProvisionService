using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using H2020.IPMDecisions.UPR.Core.Entities;
using H2020.IPMDecisions.UPR.Data.Core.Repositories;
using Microsoft.EntityFrameworkCore;

namespace H2020.IPMDecisions.UPR.Data.Persistence.Repositories
{
    internal class CropPestRepository : ICropPestRepository
    {
        private ApplicationDbContext context;

        public CropPestRepository(ApplicationDbContext context)
        {
            this.context = context;
        }

        public void Create(CropPest entity)
        {
            entity.CropEppoCode = entity.CropEppoCode.ToUpper();
            entity.PestEppoCode = entity.PestEppoCode.ToUpper();
            this.context.Add(entity);
        }

        public async Task<CropPest> FindByConditionAsync(Expression<Func<CropPest, bool>> expression)
        {
            return await this.context
                .CropPest
                .Where(expression)
                .FirstOrDefaultAsync();
        }
    }
}