using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using H2020.IPMDecisions.UPR.Core.Dtos;
using H2020.IPMDecisions.UPR.Core.Entities;
using H2020.IPMDecisions.UPR.Core.Helpers;
using H2020.IPMDecisions.UPR.Core.ResourceParameters;
using H2020.IPMDecisions.UPR.Data.Core.Repositories;
using Microsoft.EntityFrameworkCore;

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

        public async Task<PagedList<FieldSprayApplication>> FindAllAsync(FieldSprayResourceParameter resourceParameter, Guid? fieldId = null)
        {
            if (resourceParameter is null)
                throw new ArgumentNullException(nameof(resourceParameter));

            var collection = this.context.FieldSprayApplication as IQueryable<FieldSprayApplication>;

            collection = collection
                .Where(f =>
                    f.FieldCropPestId == resourceParameter.FieldCropPestId);

            collection = ApplyResourceParameter(resourceParameter, collection);

            return await PagedList<FieldSprayApplication>.CreateAsync(
                collection,
                resourceParameter.PageNumber,
                resourceParameter.PageSize);
        }

        public Task<IEnumerable<FieldSprayApplication>> FindAllAsync()
        {
            throw new NotImplementedException();
        }

        public Task<PagedList<FieldSprayApplication>> FindAllAsync(FieldSprayResourceParameter resourceParameter)
        {
            throw new NotImplementedException();
        }

        public async Task<FieldSprayApplication> FindByConditionAsync(Expression<Func<FieldSprayApplication, bool>> expression)
        {
            return await this.context
                .FieldSprayApplication
                .Where(expression)
                .FirstOrDefaultAsync();
        }

        public async Task<FieldSprayApplication> FindByConditionAsync(Expression<Func<FieldSprayApplication, bool>> expression, bool includeAssociatedData)
        {
            if (!includeAssociatedData)
            {
                return await FindByConditionAsync(expression);
            }

            return await this.context
                .FieldSprayApplication
                .Where(expression)
                .FirstOrDefaultAsync();
        }

        public async Task<FieldSprayApplication> FindByIdAsync(Guid id)
        {
            return await this
               .context
               .FieldSprayApplication
               .Where(f =>
                   f.Id == id)
               .FirstOrDefaultAsync();
        }

        public void Update(FieldSprayApplication entity)
        {
            this.context.Update(entity);
        }

        private IQueryable<FieldSprayApplication> ApplyResourceParameter(FieldSprayResourceParameter resourceParameter, IQueryable<FieldSprayApplication> collection)
        {
            if (!string.IsNullOrEmpty(resourceParameter.SearchQuery))
            {
                //ToDo: Check that Columns can do this type of query
            }
            if (!string.IsNullOrEmpty(resourceParameter.OrderBy))
            {
                //ToDo: Add PropertyMappingService
            }
            return collection;
        }
    }
}