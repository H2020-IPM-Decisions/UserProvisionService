using H2020.IPMDecisions.UPR.BLL.Helpers;
using H2020.IPMDecisions.UPR.Core.Dtos;
using H2020.IPMDecisions.UPR.Core.Entities;
using H2020.IPMDecisions.UPR.Core.Enums;
using H2020.IPMDecisions.UPR.Core.Helpers;
using H2020.IPMDecisions.UPR.Core.Models;
using H2020.IPMDecisions.UPR.Core.ResourceParameters;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Net.Http.Headers;
using NetTopologySuite.Geometries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace H2020.IPMDecisions.UPR.BLL
{
    public partial class BusinessLogic : IBusinessLogic
    {
        public async Task<GenericResponse> DeleteFarm(HttpContext httpContext)
        {
            try
            {
                var farm = httpContext.Items["farm"] as Farm;
                if (farm == null) return GenericResponseBuilder.Success();

                var userId = Guid.Parse(httpContext.Items["userId"].ToString());
                if (!farm.UserFarms.Any(uf => uf.UserId == userId & uf.UserFarmType.Id == UserFarmTypeEnum.Owner))
                {
                    return GenericResponseBuilder.NoSuccess(this.jsonStringLocalizer["farm.owner_error"].ToString());
                };

                this.dataService.Farms.Delete(farm);
                await this.dataService.CompleteAsync();

                return GenericResponseBuilder.Success();
            }
            catch (Exception ex)
            {
                logger.LogError(string.Format("Error in BLL - DeleteFarm. {0}", ex.Message));
                String innerMessage = (ex.InnerException != null) ? ex.InnerException.Message : "";
                return GenericResponseBuilder.NoSuccess($"{ex.Message} InnerException: {innerMessage}");
            }
        }

        public async Task<GenericResponse<IDictionary<string, object>>> LinkNewFarmToUserProfile(FarmForCreationDto farmForCreationDto, Guid userId, string mediaType)
        {
            try
            {
                if (!MediaTypeHeaderValue.TryParse(mediaType,
                       out MediaTypeHeaderValue parsedMediaType))
                    return GenericResponseBuilder.NoSuccess<IDictionary<string, object>>(null, "Wrong media type.");

                var userProfile = await GetUserProfileByUserId(userId);
                if (userProfile.Result == null)
                {
                    return GenericResponseBuilder.NoSuccess<IDictionary<string, object>>(null, "Please create a `User Profile` first.");
                }

                var farmAsEntity = this.mapper.Map<Farm>(farmForCreationDto);

                // Call Euroweather with Farm location to start getting Euroweather data from location
#pragma warning disable 4014
                StartEuroweatherDataCollectionProcess(farmAsEntity.Location);
#pragma warning restore 4014

                var defaultIdWeatherForecast = AdminValuesEnum.WeatherForecastService;
                var weatherForecastDefaultValue = await this.dataService.AdminVariables.FindByIdAsync(defaultIdWeatherForecast);
                if (weatherForecastDefaultValue == null) throw new ApplicationException("Database do not contain default weather forecast value.");
                var weatherForecast = await EnsureWeatherForecastExists(weatherForecastDefaultValue.Value);
                farmAsEntity.WeatherForecast = weatherForecast;

                var defaultIdWeatherhistorical = AdminValuesEnum.WeatherHistoricalService;
                var weatherHistoricalDefaultValue = await this.dataService.AdminVariables.FindByIdAsync(defaultIdWeatherhistorical);
                if (weatherHistoricalDefaultValue == null) throw new ApplicationException("Database do not contain default weather historical value.");
                var weatherHistorical = await EnsureWeatherHistoricalExists(weatherHistoricalDefaultValue.Value);
                farmAsEntity.WeatherHistorical = weatherHistorical;

                await this.dataService.UserProfiles.AddFarm(userProfile.Result, farmAsEntity, UserFarmTypeEnum.Owner, false);
                await this.dataService.CompleteAsync();

                var farmToReturn = this.mapper.Map<FarmDto>(farmAsEntity)
                    .ShapeData()
                    as IDictionary<string, object>;

                var includeLinks = parsedMediaType.SubTypeWithoutSuffix
                            .EndsWith("hateoas", StringComparison.InvariantCultureIgnoreCase);
                if (includeLinks)
                {
                    var links = UrlCreatorHelper.CreateLinksForFarm(url, farmAsEntity.Id);
                    farmToReturn.Add("links", links);
                }
                return GenericResponseBuilder.Success<IDictionary<string, object>>(farmToReturn);
            }
            catch (Exception ex)
            {
                logger.LogError(string.Format("Error in BLL - LinkNewFarmToUserProfile. {0}", ex.Message), ex);
                String innerMessage = (ex.InnerException != null) ? ex.InnerException.Message : "";
                return GenericResponseBuilder.NoSuccess<IDictionary<string, object>>(null, $"{ex.Message} InnerException: {innerMessage}");
            }
        }

        private async Task StartEuroweatherDataCollectionProcess(Point location)
        {
            try
            {
                var euroWeatherId = "net.ipmdecisions.dwd.euroweather";
                var weatherInformation = await this.internalCommunicationProvider
                      .GetWeatherProviderInformationFromWeatherMicroservice(euroWeatherId);
                var parametersUrl = string.Format("longitude={0}&latitude={1}",
                    location.X,
                    location.Y);

                // Do not wait as is not important for the process
#pragma warning disable 4014
                this.internalCommunicationProvider.GetWeatherUsingOwnService(weatherInformation.EndPoint.AbsoluteUri, parametersUrl);
#pragma warning restore 4014
            }
            catch (Exception ex)
            {

                logger.LogError(string.Format("Error in BLL - StartEuroweatherDataCollectionProcess. {0}", ex.Message), ex);
                String innerMessage = (ex.InnerException != null) ? ex.InnerException.Message : "";
            }
        }

        public async Task<GenericResponse<Farm>> GetFarm(Guid id, HttpContext httpContext)
        {
            try
            {
                var userId = Guid.Parse(httpContext.Items["userId"].ToString());
                var isAdmin = bool.Parse(httpContext.Items["isAdmin"].ToString());

                Farm farmAsEntity = await this
                        .dataService
                        .Farms
                        .FindByIdAsync(id, true);

                if (farmAsEntity == null) return GenericResponseBuilder.NotFound<Farm>();

                if (isAdmin == false)
                {
                    var belongsToUser = farmAsEntity
                        .UserFarms
                        .Any(uf => uf.UserId == userId);

                    if (!belongsToUser)
                        return GenericResponseBuilder.Unauthorized<Farm>();
                }

                return GenericResponseBuilder.Success<Farm>(farmAsEntity);
            }
            catch (Exception ex)
            {
                logger.LogError(string.Format("Error in BLL - GetFarm. {0}", ex.Message), ex);
                String innerMessage = (ex.InnerException != null) ? ex.InnerException.Message : "";
                return GenericResponseBuilder.NoSuccess<Farm>(null, $"{ex.Message} InnerException: {innerMessage}");
            }
        }

        public GenericResponse<IDictionary<string, object>> GetFarmDto(
            Guid id,
            HttpContext httpContext,
            FarmResourceParameter resourceParameter,
            string mediaType)
        {
            try
            {
                if (!MediaTypeHeaderValue.TryParse(mediaType,
                       out MediaTypeHeaderValue parsedMediaType))
                    return GenericResponseBuilder.NoSuccess<IDictionary<string, object>>(null, "Wrong media type.");

                if (!propertyCheckerService.TypeHasProperties<FarmWithChildrenDto>(resourceParameter.Fields, false))
                    return GenericResponseBuilder.NoSuccess<IDictionary<string, object>>(null, "Wrong fields entered");

                var includeLinks = parsedMediaType.SubTypeWithoutSuffix
                    .EndsWith("hateoas", StringComparison.InvariantCultureIgnoreCase);
                bool includeChildren = parsedMediaType.SubTypeWithoutSuffix.ToString().Contains("withchildren");

                var farm = httpContext.Items["farm"] as Farm;
                if (farm == null) return GenericResponseBuilder.NotFound<IDictionary<string, object>>();
                IEnumerable<LinkDto> links = new List<LinkDto>();
                if (includeLinks)
                {
                    links = UrlCreatorHelper.CreateLinksForFarm(url, id, resourceParameter.Fields);
                }

                if (includeChildren)
                {
                    var farmToReturnWithChildrenShaped = CreateFarmWithChildrenAsDictionary(
                        resourceParameter,
                        includeLinks,
                        farm,
                        links);

                    return GenericResponseBuilder.Success<IDictionary<string, object>>(farmToReturnWithChildrenShaped);
                }

                var farmToReturn = this.mapper.Map<FarmDto>(farm)
                    .ShapeData(resourceParameter.Fields) as IDictionary<string, object>;

                if (includeLinks)
                {
                    farmToReturn.Add("links", links);
                }

                return GenericResponseBuilder.Success<IDictionary<string, object>>(farmToReturn);
            }
            catch (Exception ex)
            {
                logger.LogError(string.Format("Error in BLL - GetFarmDto. {0}", ex.Message), ex);
                String innerMessage = (ex.InnerException != null) ? ex.InnerException.Message : "";
                return GenericResponseBuilder.NoSuccess<IDictionary<string, object>>(null, $"{ex.Message} InnerException: {innerMessage}");
            }
        }

        public async Task<GenericResponse<ShapedDataWithLinks>> GetFarms(Guid userId, FarmResourceParameter resourceParameter, string mediaType)
        {
            try
            {
                if (!MediaTypeHeaderValue.TryParse(mediaType,
                       out MediaTypeHeaderValue parsedMediaType))
                    return GenericResponseBuilder.NoSuccess<ShapedDataWithLinks>(null, "Wrong media type.");

                if (!propertyCheckerService.TypeHasProperties<FarmWithChildrenDto>(resourceParameter.Fields, true))
                    return GenericResponseBuilder.NoSuccess<ShapedDataWithLinks>(null, "Wrong fields entered or missing 'id' field");

                if (!propertyMappingService.ValidMappingExistsFor<FarmDto, Farm>(resourceParameter.OrderBy))
                    return GenericResponseBuilder.NoSuccess<ShapedDataWithLinks>(null, "Wrong OrderBy entered");

                var includeLinks = parsedMediaType.SubTypeWithoutSuffix
                    .EndsWith("hateoas", StringComparison.InvariantCultureIgnoreCase);
                bool includeChildren = parsedMediaType.SubTypeWithoutSuffix.ToString().Contains("withchildren");

                var farmsAsEntities = await this
                    .dataService
                    .Farms
                    .FindAllAsync(resourceParameter, userId, includeChildren);
                if (farmsAsEntities.Count == 0) return GenericResponseBuilder.NotFound<ShapedDataWithLinks>();

                var paginationMetaData = MiscellaneousHelper.CreatePaginationMetadata(farmsAsEntities);
                var paginationLinks = UrlCreatorHelper.CreateLinksForFarms(
                    url,
                    resourceParameter,
                    farmsAsEntities.HasNext,
                    farmsAsEntities.HasPrevious);

                if (includeChildren)
                {
                    var farmsAsDictionaryList = farmsAsEntities.Select(farm =>
                    {
                        return CreateFarmWithChildrenAsDictionary(
                            resourceParameter,
                            includeLinks,
                            farm,
                            paginationLinks);
                    });

                    var farmsToReturnWitChildren = new ShapedDataWithLinks()
                    {
                        Value = farmsAsDictionaryList,
                        Links = paginationLinks,
                        PaginationMetaData = paginationMetaData
                    };

                    return GenericResponseBuilder.Success<ShapedDataWithLinks>(farmsToReturnWitChildren);
                }

                var shapedFarmsToReturn = this.mapper
                   .Map<IEnumerable<FarmDto>>(farmsAsEntities)
                   .ShapeData(resourceParameter.Fields);

                var shapedFarmsToReturnWithLinks = shapedFarmsToReturn.Select(farm =>
                {
                    var farmAsDictionary = farm as IDictionary<string, object>;
                    if (includeLinks)
                    {
                        var farmsLinks = UrlCreatorHelper.CreateLinksForFarm(
                            url,
                            (Guid)farmAsDictionary["Id"],
                            resourceParameter.Fields);
                        farmAsDictionary.Add("links", farmsLinks);
                    }
                    return farmAsDictionary;
                });

                var farmsToReturn = new ShapedDataWithLinks()
                {
                    Value = shapedFarmsToReturnWithLinks,
                    Links = paginationLinks,
                    PaginationMetaData = paginationMetaData
                };

                return GenericResponseBuilder.Success<ShapedDataWithLinks>(farmsToReturn);
            }
            catch (Exception ex)
            {
                logger.LogError(string.Format("Error in BLL - GetFarms. {0}", ex.Message), ex);
                String innerMessage = (ex.InnerException != null) ? ex.InnerException.Message : "";
                return GenericResponseBuilder.NoSuccess<ShapedDataWithLinks>(null, $"{ex.Message} InnerException: {innerMessage}");
            }
        }

        public async Task<GenericResponse> FullUpdateFarm(Farm farm, FarmForFullUpdateDto farmForFullUpdate)
        {
            try
            {
                farm.WeatherForecast = await EnsureWeatherForecastExists(farmForFullUpdate.WeatherForecastDto.WeatherId);
                farm.WeatherHistorical = await EnsureWeatherHistoricalExists(farmForFullUpdate.WeatherHistoricalDto.WeatherId);

                this.mapper.Map(farmForFullUpdate, farm);
                this.dataService.Farms.Update(farm);
                await this.dataService.CompleteAsync();

                return GenericResponseBuilder.Success();
            }
            catch (Exception ex)
            {
                logger.LogError(string.Format("Error in BLL - UpdateFarm. {0}", ex.Message), ex);
                String innerMessage = (ex.InnerException != null) ? ex.InnerException.Message : "";
                return GenericResponseBuilder.NoSuccess($"{ex.Message} InnerException: {innerMessage}");
            }
        }

        #region Helpers
        private async Task<Farm> FindFarm(Guid id, Guid userId, bool isAdmin, bool includeChildren)
        {
            Farm existingFarm = null;

            if (isAdmin == false)
            {
                existingFarm = await this.dataService
                .Farms
                .FindByConditionAsync(
                    f => f.Id == id
                    &&
                    f.UserFarms.Any(uf => uf.UserId == userId), includeChildren
                );
            }
            else
            {
                existingFarm = await this.dataService
                    .Farms
                    .FindByIdAsync(id, includeChildren);
            }

            return existingFarm;
        }

        public FarmForCreationDto MapToFarmForCreation(FarmForUpdateDto farmDto)
        {
            return this.mapper.Map<FarmForCreationDto>(farmDto);
        }

        public FarmForUpdateDto MapToFarmForUpdateDto(Farm farm)
        {
            return this.mapper.Map<FarmForUpdateDto>(farm);
        }

        private IDictionary<string, object> CreateFarmWithChildrenAsDictionary(FarmResourceParameter resourceParameter, bool includeLinks, Farm farmAsEntity, IEnumerable<LinkDto> links)
        {
            try
            {
                var fieldResourceParameter = this.mapper.Map<FieldResourceParameter>(resourceParameter);

                var fieldsToReturn = ShapeFieldsAsChildren(
                    farmAsEntity,
                    fieldResourceParameter,
                    includeLinks);

                var farmToReturnWithChildren = this.mapper.Map<FarmWithChildrenDto>(farmAsEntity);
                farmToReturnWithChildren.FieldsDto = fieldsToReturn;

                var farmToReturnWithChildrenShaped = farmToReturnWithChildren
                    .ShapeData(resourceParameter.Fields)
                    as IDictionary<string, object>;

                if (includeLinks)
                {
                    farmToReturnWithChildrenShaped.Add("links", links);
                }
                return farmToReturnWithChildrenShaped;
            }
            catch (Exception ex)
            {
                logger.LogError(string.Format("Error in BLL - CreateFarmWithChildrenAsDictionary. {0}", ex.Message), ex);
                return null;
            }
        }
        #endregion
    }
}