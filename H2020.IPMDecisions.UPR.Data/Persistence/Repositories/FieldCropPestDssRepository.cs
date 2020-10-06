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
    internal class FieldCropPestDssRepository : IFieldCropPestDssRepository
    {
        private ApplicationDbContext context;

        public FieldCropPestDssRepository(ApplicationDbContext context)
        {
            this.context = context;
        }

        public void Create(FieldCropPestDss entity)
        {
            this.context.Add(entity);
        }

        public void Delete(FieldCropPestDss entity)
        {
            this.context.Remove(entity);
        }

        public Task<IEnumerable<FieldCropPestDss>> FindAllAsync()
        {
            throw new NotImplementedException();
        }

        public Task<PagedList<FieldCropPestDss>> FindAllAsync(FieldCropPestDssResourceParameter resourceParameter)
        {
            throw new NotImplementedException();
        }

        public Task<FieldCropPestDss> FindByConditionAsync(Expression<Func<FieldCropPestDss, bool>> expression)
        {
            throw new NotImplementedException();
        }

        public Task<FieldCropPestDss> FindByConditionAsync(Expression<Func<FieldCropPestDss, bool>> expression, bool includeAssociatedData)
        {
            throw new NotImplementedException();
        }

        public async Task<FieldCropPestDss> FindByIdAsync(Guid id)
        {
            return await this
                .context
                .FieldCropPestDss
                .Where(f =>
                    f.Id == id)
                .FirstOrDefaultAsync();
        }

        public void Update(FieldCropPestDss entity)
        {
            this.context.Update(entity);
        }
    }
}