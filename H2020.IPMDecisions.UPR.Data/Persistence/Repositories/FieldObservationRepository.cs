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

namespace H2020.IPMDecisions.UPR.Data.Persistence
{
    internal class FieldObservationRepository : IFieldObservationRepository
    {
        private ApplicationDbContext context;
        private IPropertyMappingService propertyMappingService;

        public FieldObservationRepository(ApplicationDbContext context, IPropertyMappingService propertyMappingService)
        {
            this.context = context;
            this.propertyMappingService = propertyMappingService;
        }

        public void Create(FieldObservation entity)
        {
            this.context.Add(entity);
        }

        public void Delete(FieldObservation entity)
        {
            this.context.Remove(entity);
        }

        public async Task<IEnumerable<FieldObservation>> FindAllAsync()
        {
            return await this.context
               .FieldObservation
               .ToListAsync<FieldObservation>();
        }

        public async Task<PagedList<FieldObservation>> FindAllAsync(FieldObservationResourceParameter resourceParameter)
        {
            if (resourceParameter is null)
                throw new ArgumentNullException(nameof(resourceParameter));

            var collection = this.context.FieldObservation as IQueryable<FieldObservation>;

            collection = ApplyResourceParameter(resourceParameter, collection);

            return await PagedList<FieldObservation>.CreateAsync(
                collection,
                resourceParameter.PageNumber,
                resourceParameter.PageSize);
        }

        public async Task<PagedList<FieldObservation>> FindAllAsync(FieldObservationResourceParameter resourceParameter, Guid? fieldId = null)
        {
            if (resourceParameter is null)
                throw new ArgumentNullException(nameof(resourceParameter));

            var collection = this.context.FieldObservation as IQueryable<FieldObservation>;

            if (!string.IsNullOrEmpty(fieldId.ToString()))
            {
                //ToDo
                // collection = collection.Where(f =>
                //     f.FieldId == fieldId);
            }

            collection = ApplyResourceParameter(resourceParameter, collection);

            return await PagedList<FieldObservation>.CreateAsync(
                collection,
                resourceParameter.PageNumber,
                resourceParameter.PageSize);
        }

        public async Task<FieldObservation> FindByConditionAsync(Expression<Func<FieldObservation, bool>> expression)
        {
            return await this.context
                .FieldObservation
                .Where(expression)
                .FirstOrDefaultAsync();
        }

        public async Task<FieldObservation> FindByConditionAsync(Expression<Func<FieldObservation, bool>> expression, bool includeAssociatedData)
        {
            if (!includeAssociatedData)
            {
                return await FindByConditionAsync(expression);
            }

            return await this.context
                .FieldObservation
                .Where(expression)
                .FirstOrDefaultAsync();
        }

        public async Task<FieldObservation> FindByIdAsync(Guid id)
        {
            return await this
               .context
               .FieldObservation
               .Where(f =>
                   f.Id == id)
               .FirstOrDefaultAsync();
        }

        public void Update(FieldObservation entity)
        {
            this.context.Update(entity);
        }


        private IQueryable<FieldObservation> ApplyResourceParameter(FieldObservationResourceParameter resourceParameter, IQueryable<FieldObservation> collection)
        {
            if (!string.IsNullOrEmpty(resourceParameter.SearchQuery))
            {
                //ToDo: Check that Columns can do this type of query
            }
            if (!string.IsNullOrEmpty(resourceParameter.OrderBy))
            {
                var propertyMappingDictionary =
                    this.propertyMappingService.GetPropertyMapping<FieldObservationDto, FieldObservation>();

                collection = collection.ApplySort(resourceParameter.OrderBy, propertyMappingDictionary);
            }
            return collection;
        }
    }
}