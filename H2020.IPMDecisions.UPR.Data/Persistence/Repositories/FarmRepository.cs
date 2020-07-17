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
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<Farm>> FindAllAsync()
        {
            return await this.context
                .Farm
                .ToListAsync<Farm>();
        }

        public Task<PagedList<Farm>> FindAllAsync(FarmResourceParameter resourceParameter)
        {
            throw new NotImplementedException();
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

            if (!string.IsNullOrEmpty(resourceParameter.SearchQuery))
            {
                //ToDo: Check that Columns can do this type of query
                // var searchQuery = resourceParameter.SearchQuery.Trim();
                // collection = collection.Where(f =>
                //     f.Name.Contains(searchQuery, StringComparison.OrdinalIgnoreCase)
                //     || f.Inf1.Contains(searchQuery, StringComparison.OrdinalIgnoreCase)
                //     || f.Inf2.Contains(searchQuery, StringComparison.OrdinalIgnoreCase));
            }

            if (!string.IsNullOrEmpty(resourceParameter.OrderBy))
            {
                //ToDo: 
                var propertyMappingDictionary =
                    this.propertyMappingService.GetPropertyMapping<FarmDto, Farm>();

                collection = collection.ApplySort(resourceParameter.OrderBy, propertyMappingDictionary);
            }

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
                .Where(f =>
                    f.Id == id)
                .FirstOrDefaultAsync();
        }

        public void Update(Farm entity)
        {
            throw new NotImplementedException();
        }
    }
}