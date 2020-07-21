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
    internal class FieldRepository : IFieldRepository
    {
        private ApplicationDbContext context;
        private IPropertyMappingService propertyMappingService;

        public FieldRepository(ApplicationDbContext context, IPropertyMappingService propertyMappingService)
        {
            this.context = context;
            this.propertyMappingService = propertyMappingService;
        }

        public void Create(Field entity)
        {
            this.context.Add(entity);
        }

        public void Delete(Field entity)
        {
            this.context.Remove(entity);
        }

        public Task<IEnumerable<Field>> FindAllAsync()
        {
            throw new NotImplementedException();
        }

        public Task<PagedList<Field>> FindAllAsync(FieldResourceParameter resourceParameter)
        {
            throw new NotImplementedException();
        }

        public Task<Field> FindByCondition(Expression<Func<Field, bool>> expression)
        {
            throw new NotImplementedException();
        }

        public Task<Field> FindByIdAsync(Guid id)
        {
            throw new NotImplementedException();
        }

        public void Update(Field entity)
        {
            this.context.Update(entity);
        }
    }
}