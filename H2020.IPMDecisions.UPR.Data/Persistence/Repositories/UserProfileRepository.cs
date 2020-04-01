using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using H2020.IPMDecisions.UPR.Core.Entities;
using H2020.IPMDecisions.UPR.Core.Helpers;
using H2020.IPMDecisions.UPR.Core.ResourceParameters;
using H2020.IPMDecisions.UPR.Core.Services;
using H2020.IPMDecisions.UPR.Data.Core.Repositories;

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
            throw new NotImplementedException();
        }

        public void Delete(UserProfile entity)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<UserProfile>> FindAllAsync()
        {
            throw new NotImplementedException();
        }

        public Task<PagedList<UserProfile>> FindAllAsync(UserProfileResourceParameter resourceParameter)
        {
            throw new NotImplementedException();
        }

        public Task<UserProfile> FindByIdAsync(Guid id)
        {
            throw new NotImplementedException();
        }

        public void Update(UserProfile entity)
        {
            throw new NotImplementedException();
        }
    }
}