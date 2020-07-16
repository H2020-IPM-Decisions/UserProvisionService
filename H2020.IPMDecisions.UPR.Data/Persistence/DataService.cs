using System;
using System.Threading.Tasks;
using H2020.IPMDecisions.UPR.Core.Services;
using H2020.IPMDecisions.UPR.Data.Core;
using H2020.IPMDecisions.UPR.Data.Core.Repositories;
using H2020.IPMDecisions.UPR.Data.Persistence.Repositories;

namespace H2020.IPMDecisions.UPR.Data.Persistence
{
    public class DataService : IDataService
    {
        private readonly ApplicationDbContext context;
        private readonly IPropertyMappingService propertyMappingService;

        private IUserProfileRepository userProfiles;
        public IUserProfileRepository UserProfiles
        {
            get
            {
                if (userProfiles == null)
                {
                    userProfiles = new UserProfileRepository(this.context, this.propertyMappingService);
                }
                return userProfiles;
            }
        }

        private IFarmRepository farms;
        public IFarmRepository Farms 
        {
            get
            {
                if (farms == null)
                {
                    farms = new FarmRepository(this.context, this.propertyMappingService);
                }
                return farms;
            }
        }

        public DataService(
            ApplicationDbContext context,
            IPropertyMappingService propertyMappingService)
        {            
            this.propertyMappingService = propertyMappingService 
                ?? throw new ArgumentNullException(nameof(propertyMappingService));
            this.context = context
                ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task CompleteAsync()
        {
            if (this.context != null)
                await this.context.SaveChangesAsync();
        }

        public void Dispose()
        {
            if (this.context != null)
                this.context.Dispose();
        }
    }
}