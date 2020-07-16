using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using H2020.IPMDecisions.UPR.Core.Entities;
using H2020.IPMDecisions.UPR.Core.Helpers;
using H2020.IPMDecisions.UPR.Core.ResourceParameters;
using H2020.IPMDecisions.UPR.Core.Services;
using H2020.IPMDecisions.UPR.Data.Core.Repositories;

namespace H2020.IPMDecisions.UPR.Data.Persistence.Repositories
{
    internal class FarmRepository : IFarmRepository
    {
        private ApplicationDbContext context;
        private IPropertyMappingService propertyMappingService;

        public FarmRepository(ApplicationDbContext context, IPropertyMappingService propertyMappingService)
        {
            this.context = context;
            this.propertyMappingService = propertyMappingService;
        }

        public void Create(Farm entity)
        {
            throw new NotImplementedException();
        }

        public void Delete(Farm entity)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Farm>> FindAllAsync()
        {
            throw new NotImplementedException();
        }

        public Task<PagedList<Farm>> FindAllAsync(FarmResourceParameter resourceParameter)
        {
            throw new NotImplementedException();
        }

        public Task<Farm> FindByCondition(Expression<Func<Farm, bool>> expression)
        {
            throw new NotImplementedException();
        }

        public Task<Farm> FindByIdAsync(Guid id)
        {
            throw new NotImplementedException();
        }

        public void Update(Farm entity)
        {
            throw new NotImplementedException();
        }
    }
}