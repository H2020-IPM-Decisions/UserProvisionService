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

        public async Task<IEnumerable<FieldCropPestDss>> FindAllAsync()
        {
            return await this
            .context
            .FieldCropPestDss
                .Include(fcpd => fcpd.CropPestDss)
            .ToListAsync();
        }

        public async Task<List<FieldCropPestDss>> FindAllAsync(Expression<Func<FieldCropPestDss, bool>> expression)
        {
            if (expression is null) return null;

            return await this.context.FieldCropPestDss
                .Include(fcpd => fcpd.CropPestDss)
                .Include(fcpd => fcpd.FieldCropPest)
                    .ThenInclude(fcp => fcp.FieldCrop)
                .Include(fcpd => fcpd.FieldCropPest)
                    .ThenInclude(fcp => fcp.CropPest)
                .Where(expression)
            .ToListAsync();
        }

        public async Task<PagedList<FieldCropPestDss>> FindAllAsync(FieldCropPestDssResourceParameter resourceParameter)
        {
            if (resourceParameter is null)
                throw new ArgumentNullException(nameof(resourceParameter));

            var collection = this.context.FieldCropPest as IQueryable<FieldCropPestDss>;
            collection = collection
                .Where(f =>
                    f.FieldCropPestId == resourceParameter.FieldCropPestId);

            return await PagedList<FieldCropPestDss>.CreateAsync(
                collection,
                resourceParameter.PageNumber,
                resourceParameter.PageSize);
        }

        public async Task<PagedList<FieldCropPestDss>> FindAllAsync(FieldCropPestDssResourceParameter resourceParameter, bool includeAssociatedData)
        {
            if (resourceParameter is null)
                throw new ArgumentNullException(nameof(resourceParameter));

            if (!includeAssociatedData)
            {
                return await FindAllAsync(resourceParameter);
            }

            var collection = this.context.FieldCropPestDss as IQueryable<FieldCropPestDss>;
            collection = collection
                .Where(f =>
                    f.FieldCropPestId == resourceParameter.FieldCropPestId)
                .Include(f => f.FieldCropPest)
                    .ThenInclude(fcp => fcp.CropPest);

            return await PagedList<FieldCropPestDss>.CreateAsync(
                collection,
                resourceParameter.PageNumber,
                resourceParameter.PageSize);
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