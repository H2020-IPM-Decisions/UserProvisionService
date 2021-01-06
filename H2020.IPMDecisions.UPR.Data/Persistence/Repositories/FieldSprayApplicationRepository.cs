using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using H2020.IPMDecisions.UPR.Core.Entities;
using H2020.IPMDecisions.UPR.Core.Helpers;
using H2020.IPMDecisions.UPR.Core.ResourceParameters;
using H2020.IPMDecisions.UPR.Data.Core.Repositories;

namespace H2020.IPMDecisions.UPR.Data.Persistence
{
    internal class FieldSprayApplicationRepository : IFieldSprayApplicationRepository
    {
        private ApplicationDbContext context;
        public FieldSprayApplicationRepository(ApplicationDbContext context)
        {
            this.context = context;
        }

        public void Create(FieldSprayApplication entity)
        {
            this.context.Add(entity);
        }

        public void Delete(FieldSprayApplication entity)
        {
            this.context.Remove(entity);
        }

        public Task<PagedList<FieldSprayApplication>> FindAllAsync(FieldSprayResourceParameter resourceParameter, Guid? fieldId = null)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<FieldSprayApplication>> FindAllAsync()
        {
            throw new NotImplementedException();
        }

        public Task<PagedList<FieldSprayApplication>> FindAllAsync(FieldSprayResourceParameter resourceParameter)
        {
            throw new NotImplementedException();
        }

        public Task<FieldSprayApplication> FindByConditionAsync(Expression<Func<FieldSprayApplication, bool>> expression)
        {
            throw new NotImplementedException();
        }

        public Task<FieldSprayApplication> FindByConditionAsync(Expression<Func<FieldSprayApplication, bool>> expression, bool includeAssociatedData)
        {
            throw new NotImplementedException();
        }

        public Task<FieldSprayApplication> FindByIdAsync(Guid id)
        {
            throw new NotImplementedException();
        }

        public void Update(FieldSprayApplication entity)
        {
            this.context.Update(entity);
        }
    }
}