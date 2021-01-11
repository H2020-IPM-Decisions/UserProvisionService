using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using H2020.IPMDecisions.UPR.Core.Dtos;
using H2020.IPMDecisions.UPR.Core.Entities;
using H2020.IPMDecisions.UPR.Core.Helpers;
using H2020.IPMDecisions.UPR.Core.ResourceParameters;
using H2020.IPMDecisions.UPR.Core.Services;
using H2020.IPMDecisions.UPR.Data.Core.Repositories;
using Microsoft.EntityFrameworkCore;

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

        public async Task<IEnumerable<Field>> FindAllAsync()
        {
            return await this.context
               .Field
               .ToListAsync<Field>();
        }

        public async Task<PagedList<Field>> FindAllAsync(FieldResourceParameter resourceParameter)
        {
            if (resourceParameter is null)
                throw new ArgumentNullException(nameof(resourceParameter));

            var collection = this.context.Field as IQueryable<Field>;

            collection = ApplyResourceParameter(resourceParameter, collection);

            return await PagedList<Field>.CreateAsync(
                collection,
                resourceParameter.PageNumber,
                resourceParameter.PageSize);
        }

        public async Task<PagedList<Field>> FindAllAsync(FieldResourceParameter resourceParameter, Guid farmId)
        {
            if (string.IsNullOrEmpty(farmId.ToString()))
            {
                return await FindAllAsync(resourceParameter);
            }

            if (resourceParameter is null)
                throw new ArgumentNullException(nameof(resourceParameter));

            var collection = this.context.Field as IQueryable<Field>;
            collection = collection.Where(f =>
                    f.FarmId == farmId);

            collection = ApplyResourceParameter(resourceParameter, collection);

            return await PagedList<Field>.CreateAsync(
                collection,
                resourceParameter.PageNumber,
                resourceParameter.PageSize);
        }

        public async Task<PagedList<Field>> FindAllAsync(FieldResourceParameter resourceParameter, Guid farmId, bool includeAssociatedData)
        {
            if (!includeAssociatedData)
            {
                return await FindAllAsync(resourceParameter, farmId);
            }

            var collection = this.context.Field as IQueryable<Field>;
            collection = collection
               .Where(f =>
                   f.FarmId == farmId)
                .Include(f => f.FieldCropPests)
                    .ThenInclude(f => f.FieldObservations)
               .Include(f => f.FieldCropPests)
                   .ThenInclude(fcp => fcp.CropPest)
                .Include(f => f.FieldCropPests)
                    .ThenInclude(fcp => fcp.FieldCropPestDsses)
                        .ThenInclude(fcpd => fcpd.CropPestDss)
                .Include(f => f.FieldCropPests)
                    .ThenInclude(f => f.FieldSprayApplications);

            collection = ApplyResourceParameter(resourceParameter, collection);

            return await PagedList<Field>.CreateAsync(
                collection,
                resourceParameter.PageNumber,
                resourceParameter.PageSize);
        }

        private IQueryable<Field> ApplyResourceParameter(FieldResourceParameter resourceParameter, IQueryable<Field> collection)
        {
            if (!string.IsNullOrEmpty(resourceParameter.SearchQuery))
            {
                var searchQuery = resourceParameter.SearchQuery.Trim().ToLower();
                collection = collection.Where(f =>
                    f.Name.ToLower().Contains(searchQuery)
                    || f.Inf1.ToLower().Contains(searchQuery)
                    || f.Inf2.ToLower().Contains(searchQuery));
            }
            if (!string.IsNullOrEmpty(resourceParameter.OrderBy))
            {
                var propertyMappingDictionary =
                    this.propertyMappingService.GetPropertyMapping<FieldDto, Field>();

                collection = collection.ApplySort(resourceParameter.OrderBy, propertyMappingDictionary);
            }
            return collection;
        }

        public async Task<Field> FindByConditionAsync(Expression<Func<Field, bool>> expression)
        {
            return await this.context
                .Field
                .Where(expression)
                .FirstOrDefaultAsync();
        }

        public async Task<Field> FindByConditionAsync(Expression<Func<Field, bool>> expression, bool includeAssociatedData)
        {
            if (!includeAssociatedData)
            {
                return await FindByConditionAsync(expression);
            }

            return await this.context
                .Field
                .Where(expression)
                .Include(f => f.FieldCropPests)
                    .ThenInclude(f => f.FieldObservations)
                .Include(f => f.FieldCropPests)
                    .ThenInclude(f => f.CropPest)
                .Include(f => f.FieldCropPests)
                    .ThenInclude(fcp => fcp.FieldCropPestDsses)
                        .ThenInclude(fcpd => fcpd.CropPestDss)
                .Include(f => f.FieldCropPests)
                    .ThenInclude(f => f.FieldSprayApplications)
                .FirstOrDefaultAsync();
        }

        public async Task<Field> FindByIdAsync(Guid id)
        {
            return await this
                .context
                .Field
                .Where(f =>
                    f.Id == id)
                .FirstOrDefaultAsync();
        }

        public async Task<Field> FindByIdAsync(Guid id, bool includeAssociatedData)
        {
            if (!includeAssociatedData)
            {
                return await FindByIdAsync(id);
            }

            return await this
                .context
                .Field
                .Where(f =>
                    f.Id == id)
                .Include(f => f.FieldCropPests)
                    .ThenInclude(f => f.FieldObservations)
                .Include(f => f.FieldCropPests)
                   .ThenInclude(fcp => fcp.CropPest)
                .Include(f => f.FieldCropPests)
                    .ThenInclude(f => f.FieldSprayApplications)
                .FirstOrDefaultAsync();
        }

        public void Update(Field entity)
        {
            this.context.Update(entity);
        }
    }
}