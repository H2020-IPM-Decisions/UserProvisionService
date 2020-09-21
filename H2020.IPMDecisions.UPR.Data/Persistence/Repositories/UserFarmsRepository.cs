using System.Collections.Generic;
using H2020.IPMDecisions.UPR.Core.Entities;
using H2020.IPMDecisions.UPR.Data.Core.Repositories;

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
    }
}