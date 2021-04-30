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

        private IFieldRepository fields;
        public IFieldRepository Fields
        {
            get
            {
                if (fields == null)
                {
                    fields = new FieldRepository(this.context, this.propertyMappingService);
                }
                return fields;
            }
        }

        private IFieldObservationRepository fieldObservations;
        public IFieldObservationRepository FieldObservations
        {
            get
            {
                if (fieldObservations == null)
                {
                    fieldObservations = new FieldObservationRepository(this.context, this.propertyMappingService);
                }
                return fieldObservations;
            }
        }

        private IDataShareRequestRepository dataShareRequests;
        public IDataShareRequestRepository DataShareRequests
        {
            get
            {
                if (dataShareRequests == null)
                {
                    dataShareRequests = new DataShareRequestRepository(this.context, this.propertyMappingService);
                }
                return dataShareRequests;
            }
        }

        private IDataSharingRequestStatusRepository dataSharingRequestStatuses;
        public IDataSharingRequestStatusRepository DataSharingRequestStatuses
        {
            get
            {
                if (dataSharingRequestStatuses == null)
                {
                    dataSharingRequestStatuses = new DataSharingRequestStatusRepository(this.context);
                }
                return dataSharingRequestStatuses;
            }
        }

        private IUserFarmsRepository userFarms;
        public IUserFarmsRepository UserFarms
        {
            get
            {
                if (userFarms == null)
                {
                    userFarms = new UserFarmsRepository(this.context);
                }
                return userFarms;
            }
        }

        private ICropPestRepository cropPests;
        public ICropPestRepository CropPests
        {
            get
            {
                if (cropPests == null)
                {
                    cropPests = new CropPestRepository(this.context);
                }
                return cropPests;
            }
        }

        private IFieldCropPestRepository fieldCropPests;
        public IFieldCropPestRepository FieldCropPests
        {
            get
            {
                if (fieldCropPests == null)
                {
                    fieldCropPests = new FieldCropPestRepository(this.context);
                }
                return fieldCropPests;
            }
        }

        private ICropPestDssRepository cropPestDsses;
        public ICropPestDssRepository CropPestDsses
        {
            get
            {
                if (cropPestDsses == null)
                {
                    cropPestDsses = new CropPestDssRepository(this.context);
                }
                return cropPestDsses;
            }
        }

        private IFieldCropPestDssRepository fieldCropPestDsses;
        public IFieldCropPestDssRepository FieldCropPestDsses
        {
            get
            {
                if (fieldCropPestDsses == null)
                {
                    fieldCropPestDsses = new FieldCropPestDssRepository(this.context);
                }
                return fieldCropPestDsses;
            }
        }

        private IWeatherDataSourcesRepository weatherDataSources;
        public IWeatherDataSourcesRepository WeatherDataSources
        {
            get
            {
                if (weatherDataSources == null)
                {
                    weatherDataSources = new WeatherDataSourcesRepository(this.context);
                }
                return weatherDataSources;
            }
        }

        private IWeatherHistoricalRepository weatherHistoricals;
        public IWeatherHistoricalRepository WeatherHistoricals
        {
            get
            {
                if (weatherHistoricals == null)
                {
                    weatherHistoricals = new WeatherHistoricalRepository(this.context);
                }
                return weatherHistoricals;
            }
        }

        private IFieldSprayApplicationRepository fieldSprayApplication;
        public IFieldSprayApplicationRepository FieldSprayApplication
        {
            get
            {
                if (fieldSprayApplication == null)
                {
                    fieldSprayApplication = new FieldSprayApplicationRepository(this.context);
                }
                return fieldSprayApplication;
            }
        }

        private IUserWidgetRepository userWidgets;
        public IUserWidgetRepository UserWidgets
        {
            get
            {
                if (userWidgets == null)
                {
                    userWidgets = new UserWidgetRepository(this.context);
                }
                return userWidgets;
            }
        }

        public IWeatherForecastRepository weatherForecasts;

        public IWeatherForecastRepository WeatherForecasts
        {
            get
            {
                if (weatherForecasts == null)
                {
                    weatherForecasts = new WeatherForecastRepository(this.context);
                }
                return weatherForecasts;
            }
        }

        public IDssResultRepository dssResult;
        public IDssResultRepository DssResult
        {
            get
            {
                if (dssResult == null)
                {
                    dssResult = new DssResultRepository(this.context);
                }
                return dssResult;
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