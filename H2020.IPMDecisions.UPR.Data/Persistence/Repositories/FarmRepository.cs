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

        public async Task<PagedList<Farm>> FindAllAsync(FarmResourceParameter resourceParameter, Guid? userId = null)
        {
            if (resourceParameter is null)
                throw new ArgumentNullException(nameof(resourceParameter));

            var collection = this.context.Farm as IQueryable<Farm>;

            if (!string.IsNullOrEmpty(userId.ToString()))
            {
                collection = collection.Where(f =>
                    f.UserFarms.Any
                        (uf => uf.UserId == userId));
            }

            collection = ApplyResourceParameter(resourceParameter, collection);

            return await PagedList<Farm>.CreateAsync(
                collection,
                resourceParameter.PageNumber,
                resourceParameter.PageSize);
        }

        public async Task<Farm> FindByCondition(Expression<Func<Farm, bool>> expression)
        {
            return await this.context
                .Farm
                .Where(expression)
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

        public void Update(Farm entity)
        {
            this.context.Farm.Update(entity);
        }

        #region Helpers
        private IQueryable<Farm> ApplyResourceParameter(FarmResourceParameter resourceParameter, IQueryable<Farm> collection)
        {
            if (!string.IsNullOrEmpty(resourceParameter.SearchQuery))
            {
                //ToDo: Check that Columns can do this type of query
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