using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using H2020.IPMDecisions.UPR.Core.Entities;
using H2020.IPMDecisions.UPR.Data.Core.Repositories;
using Microsoft.EntityFrameworkCore;

namespace H2020.IPMDecisions.UPR.Data.Persistence.Repositories
{
    internal class DataSharingRequestStatusRepository : IDataSharingRequestStatusRepository
    {
        private ApplicationDbContext context;

        public DataSharingRequestStatusRepository(ApplicationDbContext context)
        {
            this.context = context;
        }

        public async Task<DataSharingRequestStatus> FindByCondition(Expression<Func<DataSharingRequestStatus, bool>> expression)
        {
            return await this.context
                .DataSharingRequestStatus
                .Where(expression)
                .FirstOrDefaultAsync();
        }
    }
}