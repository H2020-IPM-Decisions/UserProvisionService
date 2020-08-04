using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using H2020.IPMDecisions.UPR.Core.Entities;
using H2020.IPMDecisions.UPR.Core.Enums;
using H2020.IPMDecisions.UPR.Core.Helpers;
using H2020.IPMDecisions.UPR.Core.ResourceParameters;
using H2020.IPMDecisions.UPR.Data.Core.Repositories;
using Microsoft.EntityFrameworkCore;

namespace H2020.IPMDecisions.UPR.Data.Persistence.Repositories
{
    internal class DataShareRequestRepository : IDataShareRequestRepository
    {
        private ApplicationDbContext context;

        public DataShareRequestRepository(ApplicationDbContext context)
        {
            this.context = context;
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

        public async Task<PagedList<DataSharingRequest>> FindAllAsync(BaseResourceParameter resourceParameter)
        {
            if (resourceParameter is null)
                throw new ArgumentNullException(nameof(resourceParameter));

            var collection = this.context.DataSharingRequest as IQueryable<DataSharingRequest>;

            return await PagedList<DataSharingRequest>.CreateAsync(
                collection,
                resourceParameter.PageNumber,
                resourceParameter.PageSize);
        }

        public async Task<DataSharingRequest> FindByCondition(Expression<Func<DataSharingRequest, bool>> expression)
        {
            return await this.context
                .DataSharingRequest
                .Where(expression)
                .FirstOrDefaultAsync();
        }

        public async Task<DataSharingRequest> FindByCondition(Expression<Func<DataSharingRequest, bool>> expression, bool includeAssociatedData)
        {
            if (!includeAssociatedData)
            {
                return await FindByCondition(expression);
            }
            return await this.context
                .DataSharingRequest
                .Where(expression)
                .Include(d => d.RequestStatus)
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
    }
}