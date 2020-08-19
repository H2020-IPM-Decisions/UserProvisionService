using System;
using System.Linq.Expressions;
using System.Threading.Tasks;
using H2020.IPMDecisions.UPR.Core.Entities;
using H2020.IPMDecisions.UPR.Data.Core.Repositories;

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
            throw new NotImplementedException();
        }

        public Task<CropPest> FindByConditionAsync(Expression<Func<CropPest, bool>> expression)
        {
            throw new NotImplementedException();
        }
    }
}