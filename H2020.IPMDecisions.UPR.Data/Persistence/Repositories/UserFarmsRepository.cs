using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using H2020.IPMDecisions.UPR.Core.Entities;
using H2020.IPMDecisions.UPR.Data.Core.Repositories;
using Microsoft.EntityFrameworkCore;

namespace H2020.IPMDecisions.UPR.Data.Persistence.Repositories
{
    internal class UserFarmsRepository : IUserFarmsRepository
    {
        private ApplicationDbContext context;

        public UserFarmsRepository(ApplicationDbContext context)
        {
            this.context = context;
        }

        public void Delete(UserFarm entity)
        {
            this.context.UserFarm.Remove(entity);
        }

        public void DeleteRange(List<UserFarm> entities)
        {
            this.context.UserFarm.RemoveRange(entities);
        }

        public async Task<IEnumerable<UserFarm>> FindAllAsync(Expression<Func<UserFarm, bool>> expression)
        {
            return await this.context
                .UserFarm
                .Where(expression)
                .Include(u => u.Farm)
                .ToListAsync();
        }

        public async Task<List<UserFarm>> GetReportDataAsync()
        {
            return await this.context
                .UserFarm
                .Where(u => u.Authorised)
                .Include(u => u.Farm)
                    .ThenInclude(f => f.Fields)
                        .ThenInclude(fi => fi.FieldCrop)
                            .ThenInclude(fc => fc.FieldCropPests)
                                .ThenInclude(fcp => fcp.CropPest)
                                    .ThenInclude(cp => cp.CropPestDsses)
                .ToListAsync();
        }
    }
}