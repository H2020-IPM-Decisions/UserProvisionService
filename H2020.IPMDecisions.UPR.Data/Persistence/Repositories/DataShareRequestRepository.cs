using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using H2020.IPMDecisions.UPR.Core.Dtos;
using H2020.IPMDecisions.UPR.Core.Entities;
using H2020.IPMDecisions.UPR.Core.Enums;
using H2020.IPMDecisions.UPR.Core.Helpers;
using H2020.IPMDecisions.UPR.Core.ResourceParameters;
using H2020.IPMDecisions.UPR.Core.Services;
using H2020.IPMDecisions.UPR.Data.Core.Repositories;
using Microsoft.EntityFrameworkCore;

namespace H2020.IPMDecisions.UPR.Data.Persistence.Repositories
{
    internal class DataShareRequestRepository : IDataShareRequestRepository
    {
        private ApplicationDbContext context;
        private IPropertyMappingService propertyMappingService;
        public DataShareRequestRepository(ApplicationDbContext context, IPropertyMappingService propertyMappingService)
        {
            this.context = context
                ?? throw new ArgumentNullException(nameof(context));
            this.propertyMappingService = propertyMappingService
            ?? throw new ArgumentNullException(nameof(propertyMappingService));
        }

        public void Create(DataSharingRequest entity)
        {
            this.context.Add(entity);
        }

        public async Task Create(Guid requesterId, Guid requesteeId, RequestStatusEnum requestStatus)
        {
            var statusFromDb = await this
                .context
                .DataSharingRequestStatus
                .FirstOrDefaultAsync(d => d.Description.Equals(requestStatus.ToString()));

            var dataShareRequest = new DataSharingRequest()
            {
                RequesteeId = requesteeId,
                RequesterId = requesterId,
                RequestStatus = statusFromDb
            };
            Create(dataShareRequest);
        }

        public void Delete(DataSharingRequest entity)
        {
            this.context.Remove(entity);
        }

        public async Task<IEnumerable<DataSharingRequest>> FindAllAsync()
        {
            return await this.context
                .DataSharingRequest
                .ToListAsync<DataSharingRequest>();
        }

        public async Task<PagedList<DataSharingRequest>> FindAllAsync(DataShareResourceParameter resourceParameter)
        {
            if (resourceParameter is null)
                throw new ArgumentNullException(nameof(resourceParameter));

            var collection = this.context.DataSharingRequest as IQueryable<DataSharingRequest>;

            collection = collection
                .Include(d => d.RequestStatus)
                .Include(d => d.Requestee)
                .Include(d => d.Requester);

            collection = ApplyResourceParameter(resourceParameter, collection);

            return await PagedList<DataSharingRequest>.CreateAsync(
                collection,
                resourceParameter.PageNumber,
                resourceParameter.PageSize);
        }

        public async Task<PagedList<DataSharingRequest>> FindAllAsync(Guid userId, DataShareResourceParameter resourceParameter)
        {
            if (string.IsNullOrEmpty(userId.ToString()))
            {
                return await FindAllAsync(resourceParameter);
            }

            if (resourceParameter is null)
                throw new ArgumentNullException(nameof(resourceParameter));

            var collection = this.context.DataSharingRequest as IQueryable<DataSharingRequest>;

            collection = collection.Where(d => d.RequesteeId == userId
            || d.RequesterId == userId);

            collection = collection
                .Include(d => d.RequestStatus)
                .Include(d => d.Requestee)
                .Include(d => d.Requester);

            collection = ApplyResourceParameter(resourceParameter, collection);

            return await PagedList<DataSharingRequest>.CreateAsync(
                collection,
                resourceParameter.PageNumber,
                resourceParameter.PageSize);
        }

        public async Task<DataSharingRequest> FindByConditionAsync(Expression<Func<DataSharingRequest, bool>> expression)
        {
            return await this.context
                .DataSharingRequest
                .Where(expression)
                .FirstOrDefaultAsync();
        }

        public async Task<DataSharingRequest> FindByConditionAsync(Expression<Func<DataSharingRequest, bool>> expression, bool includeAssociatedData)
        {
            if (!includeAssociatedData)
            {
                return await FindByConditionAsync(expression);
            }
            return await this.context
                .DataSharingRequest
                .Where(expression)
                .Include(d => d.RequestStatus)
                .Include(d => d.Requestee)
                    .ThenInclude(u => u.UserFarms)
                        .ThenInclude(uf => uf.Farm)
                .FirstOrDefaultAsync();
        }

        public async Task<DataSharingRequest> FindByIdAsync(Guid id)
        {
            return await this
                .context
                .DataSharingRequest
                .Where(d =>
                    d.Id == id)
                .FirstOrDefaultAsync();
        }

        public void Update(DataSharingRequest entity)
        {
            this.context.Update(entity);
        }

        #region Helpers
        private IQueryable<DataSharingRequest> ApplyResourceParameter(DataShareResourceParameter resourceParameter, IQueryable<DataSharingRequest> collection)
        {
            if (!string.IsNullOrEmpty(resourceParameter.RequestStatus))
            {
                collection = collection.Where(d =>
                    d.RequestStatus.Description.ToLower() == resourceParameter.RequestStatus.ToLower().Trim());                   
            }

            if (!string.IsNullOrEmpty(resourceParameter.OrderBy))
            {
                var propertyMappingDictionary =
                    this.propertyMappingService.GetPropertyMapping<DataShareRequestDto, DataSharingRequest>();

                collection = collection.ApplySort(resourceParameter.OrderBy, propertyMappingDictionary);
            }
            return collection;
        }
        #endregion
    }
}