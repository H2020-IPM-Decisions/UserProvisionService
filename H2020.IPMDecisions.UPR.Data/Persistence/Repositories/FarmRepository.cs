using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
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
            this.context.Farm.Remove(entity);
        }

        public async Task<IEnumerable<Farm>> FindAllAsync()
        {
            return await this.context
                .Farm
                .Include(f => f.UserFarms)
                .ToListAsync<Farm>();
        }

        public async Task<IEnumerable<Farm>> FindAllByConditionAsync(Expression<Func<Farm, bool>> expression)
        {
            return await this.context
                .Farm
                .Where(expression)
                .Include(f => f.Fields)
                    .ThenInclude(fi => fi.FieldCrop)
                .Include(f => f.WeatherForecast)
                .Include(f => f.WeatherHistorical)
                .Include(f => f.UserFarms)
                    .ThenInclude(uf => uf.UserFarmType)
                .Include(f => f.UserFarms)
                    .ThenInclude(uf => uf.UserProfile)
                .ToListAsync<Farm>();
        }

        public async Task<PagedList<Farm>> FindAllAsync(FarmResourceParameter resourceParameter)
        {
            if (resourceParameter is null)
                throw new ArgumentNullException(nameof(resourceParameter));

            var collection = this.context.Farm as IQueryable<Farm>;

            collection = ApplyResourceParameter(resourceParameter, collection);

            return await PagedList<Farm>.CreateAsync(
                collection,
                resourceParameter.PageNumber,
                resourceParameter.PageSize);
        }

        public async Task<PagedList<Farm>> FindAllAsync(FarmResourceParameter resourceParameter, Guid userId)
        {
            if (string.IsNullOrEmpty(userId.ToString()))
            {
                return await FindAllAsync(resourceParameter);
            }

            if (resourceParameter is null)
                throw new ArgumentNullException(nameof(resourceParameter));

            var collection = this.context.Farm as IQueryable<Farm>;
            collection = collection
                .Where(f =>
                    f.UserFarms.Any
                        (uf => uf.UserId == userId));

            collection = collection
                .Include(f => f.WeatherForecast)
                .Include(f => f.WeatherHistorical)
                .Include(f => f.UserFarms)
                    .ThenInclude(uf => uf.UserFarmType)
                .Include(f => f.UserFarms)
                    .ThenInclude(uf => uf.UserProfile);

            collection = ApplyResourceParameter(resourceParameter, collection);

            return await PagedList<Farm>.CreateAsync(
                collection,
                resourceParameter.PageNumber,
                resourceParameter.PageSize);
        }

        public async Task<PagedList<Farm>> FindAllAsync(FarmResourceParameter resourceParameter, Guid userId, bool includeAssociatedData)
        {
            if (!includeAssociatedData)
            {
                return await FindAllAsync(resourceParameter, userId);
            }

            if (resourceParameter is null)
                throw new ArgumentNullException(nameof(resourceParameter));

            var collection = this.context.Farm as IQueryable<Farm>;
            collection = collection.Where(f =>
                    f.UserFarms.Any
                        (uf => uf.UserId == userId));

            collection = collection
                .Include(f => f.WeatherForecast)
                .Include(f => f.WeatherHistorical)
                .Include(f => f.Fields)
                .Include(f => f.UserFarms);
            // .ThenInclude(fi => fi.FieldObservations);

            collection = ApplyResourceParameter(resourceParameter, collection);

            return await PagedList<Farm>.CreateAsync(
                collection,
                resourceParameter.PageNumber,
                resourceParameter.PageSize);
        }

        public async Task<Farm> FindByConditionAsync(Expression<Func<Farm, bool>> expression)
        {
            return await this.context
                .Farm
                .Where(expression)
                .FirstOrDefaultAsync();
        }

        public async Task<Farm> FindByConditionAsync(Expression<Func<Farm, bool>> expression, bool includeAssociatedData)
        {
            if (!includeAssociatedData)
            {
                return await FindByConditionAsync(expression);
            }
            return await this.context
                .Farm
                .Where(expression)
                .Include(f => f.UserFarms)
                    .ThenInclude(fu => fu.UserFarmType)
                .Include(f => f.Fields)
                    .ThenInclude(fi => fi.FieldCrop)
                        .ThenInclude(fc => fc.FieldCropPests)
                            .ThenInclude(fcp => fcp.CropPest)
                 .Include(f => f.Fields)
                    .ThenInclude(fi => fi.FieldCrop)
                        .ThenInclude(fc => fc.FieldCropPests)
                            .ThenInclude(fcp => fcp.FieldCropPestDsses)
                                .ThenInclude(fcpd => fcpd.CropPestDss)
                .Include(f => f.WeatherForecast)
                .Include(f => f.WeatherHistorical)
                .FirstOrDefaultAsync();
        }

        public async Task<Farm> FindByIdAsync(Guid id)
        {
            return await this
                .context
                .Farm
                .Include(f => f.UserFarms)
                .Where(f =>
                    f.Id == id)
                .FirstOrDefaultAsync();
        }

        public async Task<Farm> FindByIdAsync(Guid id, bool includeAssociatedData)
        {
            if (!includeAssociatedData)
            {
                return await FindByIdAsync(id);
            }

            return await this
                .context
                .Farm
                .Include(f => f.UserFarms)
                .Include(f => f.Fields)
                    .ThenInclude(fi => fi.FieldCrop)
                        .ThenInclude(fc => fc.FieldCropPests)
                            .ThenInclude(fcp => fcp.CropPest)
                 .Include(f => f.Fields)
                    .ThenInclude(fi => fi.FieldCrop)
                        .ThenInclude(fc => fc.FieldCropPests)
                            .ThenInclude(fcp => fcp.FieldCropPestDsses)
                                .ThenInclude(fcpd => fcpd.CropPestDss)
                .Include(f => f.WeatherForecast)
                .Include(f => f.WeatherHistorical)
                .Where(f =>
                    f.Id == id)
                .FirstOrDefaultAsync();
        }

        public void Update(Farm entity)
        {
            this.context.Farm.Update(entity);
        }

        #region Helpers
        private IQueryable<Farm> ApplyResourceParameter(FarmResourceParameter resourceParameter, IQueryable<Farm> collection)
        {
            if (!string.IsNullOrEmpty(resourceParameter.SearchQuery))
            {
                var searchQuery = resourceParameter.SearchQuery.Trim().ToLower();
                collection = collection.Where(f =>
                    f.Name.ToLower().Contains(searchQuery));
            }
            if (!string.IsNullOrEmpty(resourceParameter.OrderBy))
            {
                var propertyMappingDictionary =
                    this.propertyMappingService.GetPropertyMapping<FarmDto, Farm>();

                collection = collection.ApplySort(resourceParameter.OrderBy, propertyMappingDictionary);
            }
            return collection;
        }
        #endregion
    }
}