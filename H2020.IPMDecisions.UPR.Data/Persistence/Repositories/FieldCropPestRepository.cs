using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using H2020.IPMDecisions.UPR.Core.Entities;
using H2020.IPMDecisions.UPR.Core.Helpers;
using H2020.IPMDecisions.UPR.Core.ResourceParameters;
using H2020.IPMDecisions.UPR.Data.Core.Repositories;
using Microsoft.EntityFrameworkCore;

namespace H2020.IPMDecisions.UPR.Data.Persistence.Repositories
{
    internal class FieldCropPestRepository : IFieldCropPestRepository
    {
        private ApplicationDbContext context;

        public FieldCropPestRepository(ApplicationDbContext context)
        {
            this.context = context;
        }

        public void Create(FieldCropPest entity)
        {
            this.context.Add(entity);
        }

        public void Delete(FieldCropPest entity)
        {
            this.context.Remove(entity);
        }

        public async Task<IEnumerable<FieldCropPest>> FindAllAsync()
        {
            return await this.context
               .FieldCropPest
               .ToListAsync<FieldCropPest>();
        }

        public async Task<PagedList<FieldCropPest>> FindAllAsync(FieldCropPestResourceParameter resourceParameter)
        {
            if (resourceParameter is null)
                throw new ArgumentNullException(nameof(resourceParameter));

            var collection = this.context.FieldCropPest as IQueryable<FieldCropPest>;

            return await PagedList<FieldCropPest>.CreateAsync(
                collection,
                resourceParameter.PageNumber,
                resourceParameter.PageSize);
        }

        public async Task<FieldCropPest> FindByConditionAsync(Expression<Func<FieldCropPest, bool>> expression)
        {
            return await this.context
               .FieldCropPest
               .Where(expression)
               .FirstOrDefaultAsync();
        }

        public Task<FieldCropPest> FindByConditionAsync(Expression<Func<FieldCropPest, bool>> expression, bool includeAssociatedData)
        {
            throw new NotImplementedException();
        }

        public Task<FieldCropPest> FindByIdAsync(Guid id)
        {
            throw new NotImplementedException();
        }

        public void Update(FieldCropPest entity)
        {
            this.context.Update(entity);
        }
    }
}