using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using H2020.IPMDecisions.UPR.Core.Entities;
using H2020.IPMDecisions.UPR.Core.Helpers;
using H2020.IPMDecisions.UPR.Core.ResourceParameters;
using H2020.IPMDecisions.UPR.Core.Services;
using H2020.IPMDecisions.UPR.Data.Core.Repositories;
using Microsoft.EntityFrameworkCore;

namespace H2020.IPMDecisions.UPR.Data.Persistence.Repositories
{
    internal class UserProfileRepository : IUserProfileRepository
    {
        private ApplicationDbContext context;
        private IPropertyMappingService propertyMappingService;

        public UserProfileRepository(ApplicationDbContext context, IPropertyMappingService propertyMappingService)
        {
            this.context = context;
            this.propertyMappingService = propertyMappingService;
        }

        public void Create(UserProfile entity)
        {
            this.context.UserProfile.Add(entity);
        }

        public void Delete(UserProfile entity)
        {
            this.context.UserAddress.RemoveRange(entity.UserAddress);
            this.context.UserProfile.Remove(entity);
        }

        public Task<IEnumerable<UserProfile>> FindAllAsync()
        {
            throw new NotImplementedException();
        }

        public Task<PagedList<UserProfile>> FindAllAsync(UserProfileResourceParameter resourceParameter)
        {
            throw new NotImplementedException();
        }

        public async Task<UserProfile> FindByCondition(Expression<Func<UserProfile, bool>> expression)
        {
            return await this.context
                .UserProfile
                .Where(expression)
                .Include(u => u.UserAddress)
                .FirstOrDefaultAsync();
        }
        
        public async Task<UserProfile> FindByCondition(Expression<Func<UserProfile, bool>> expression, bool includeAssociatedData)
        {
            if (includeAssociatedData)
            {
                return await FindByCondition(expression);
            }

            return await this.context
                .UserProfile
                .Where(expression)
                .Include(u => u.UserAddress)
                .Include(u => u.UserFarms)
                    .ThenInclude(uf => uf.Farm)
                .FirstOrDefaultAsync();
        }

        public async Task<UserProfile> FindByIdAsync(Guid id)
        {
            return await this.context
                .UserProfile
                .Include(u => u.UserAddress)
                .SingleOrDefaultAsync(a =>
                    a.Id == id);
        }

        public void Update(UserProfile entity)
        {
            this.context.UserProfile.Update(entity);
        }

        public void AddFarm(UserProfile userProfile, Farm farm)
        {
            userProfile.UserFarms = new List<UserFarm>
            {
                new UserFarm
                {
                    UserProfile = userProfile,
                    Farm = farm
                }
            };
        }
    }
}