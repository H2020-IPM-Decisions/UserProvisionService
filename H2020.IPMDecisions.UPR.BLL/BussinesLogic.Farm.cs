using H2020.IPMDecisions.UPR.BLL.Helpers;
using H2020.IPMDecisions.UPR.Core.Dtos;
using H2020.IPMDecisions.UPR.Core.Entities;
using H2020.IPMDecisions.UPR.Core.Helpers;
using H2020.IPMDecisions.UPR.Core.Models;
using H2020.IPMDecisions.UPR.Core.ResourceParameters;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Net.Http.Headers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace H2020.IPMDecisions.UPR.BLL
{
    public partial class BusinessLogic : IBusinessLogic
    {
        public async Task<GenericResponse> DeleteFarm(Guid id, HttpContext httpContext)
        {
            try
            {
                var userId = Guid.Parse(httpContext.Items["userId"].ToString());
                var isAdmin = httpContext.Items["isAdmin"];

                Farm existingFarm = await FindFarm(id, userId, isAdmin, false);

                if (existingFarm == null) return GenericResponseBuilder.Success();

                this.dataService.Farms.Delete(existingFarm);
                await this.dataService.CompleteAsync();

                return GenericResponseBuilder.Success();
            }
            catch (Exception ex)
            {
                logger.LogError(string.Format("Error in BLL - DeleteFarm. {0}", ex.Message));
                return GenericResponseBuilder.NoSuccess($"{ex.Message} InnerException: {ex.InnerException.Message}");
            }
        }

        public async Task<GenericResponse<IDictionary<string, object>>> LinkNewFarmToUserProfile(FarmForCreationDto farmForCreationDto, Guid id, Guid userId)
        {
            try
            {
                var userProfile = await GetUserProfile(userId);
                if (userProfile.Result == null)
                {
                    // ToDo Create empty profile
                    return GenericResponseBuilder.NoSuccess<IDictionary<string, object>>(null, "Please add User Profile first.");
                }

                var farmAsEntity = this.mapper.Map<Farm>(farmForCreationDto);
                farmAsEntity.Id = id;

                this.dataService.UserProfiles.AddFarm(userProfile.Result, farmAsEntity);
                await this.dataService.CompleteAsync();

                var farmToReturn = this.mapper.Map<FarmDto>(farmAsEntity)
                    .ShapeData()
                    as IDictionary<string, object>;

                return GenericResponseBuilder.Success<IDictionary<string, object>>(farmToReturn);
            }
            catch (Exception ex)
            {
                logger.LogError(string.Format("Error in BLL - LinkNewFarmToUserProfile. {0}", ex.Message), ex);
                return GenericResponseBuilder.NoSuccess<IDictionary<string, object>>(null, $"{ex.Message} InnerException: {ex.InnerException.Message}");
            }
        }

        public async Task<GenericResponse<IDictionary<string, object>>> LinkNewFarmToUserProfile(FarmForCreationDto farmForCreationDto, Guid userId, string mediaType)
        {
            try
            {
                if (!MediaTypeHeaderValue.TryParse(mediaType,
                       out MediaTypeHeaderValue parsedMediaType))
                    return GenericResponseBuilder.NoSuccess<IDictionary<string, object>>(null, "Wrong media type.");

                var userProfile = await GetUserProfile(userId);
                if (userProfile.Result == null)
                {
                    // ToDo Create empty profile
                    return GenericResponseBuilder.NoSuccess<IDictionary<string, object>>(null, "Please add User Profile first.");
                }
                var farmAsEntity = this.mapper.Map<Farm>(farmForCreationDto);

                this.dataService.UserProfiles.AddFarm(userProfile.Result, farmAsEntity);
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
                return GenericResponseBuilder.NoSuccess<IDictionary<string, object>>(null, $"{ex.Message} InnerException: {ex.InnerException.Message}");
            }
        }

        public async Task<GenericResponse<Farm>> GetFarm(Guid id, HttpContext httpContext)
        {
            try
            {
                var userId = Guid.Parse(httpContext.Items["userId"].ToString());
                var isAdmin = httpContext.Items["isAdmin"];

                Farm farmAsEntity = await this
                        .dataService
                        .Farms
                        .FindByIdAsync(id);

                if (farmAsEntity == null) return GenericResponseBuilder.NotFound<Farm>();

                if (isAdmin == null)
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
                return GenericResponseBuilder.NoSuccess<Farm>(null, $"{ex.Message} InnerException: {ex.InnerException.Message}");
            }
        }

        public async Task<GenericResponse<IDictionary<string, object>>> GetFarmDto(
            Guid id,
            HttpContext httpContext,
            ChildrenResourceParameter childrenResourceParameter,
            string mediaType)
        {
            try
            {
                if (!MediaTypeHeaderValue.TryParse(mediaType,
                       out MediaTypeHeaderValue parsedMediaType))
                    return GenericResponseBuilder.NoSuccess<IDictionary<string, object>>(null, "Wrong media type.");

                if (!propertyCheckerService.TypeHasProperties<FarmDto>(childrenResourceParameter.Fields, false))
                    return GenericResponseBuilder.NoSuccess<IDictionary<string, object>>(null, "Wrong fields entered");

                var userId = Guid.Parse(httpContext.Items["userId"].ToString());
                var isAdmin = httpContext.Items["isAdmin"];

                var includeLinks = parsedMediaType.SubTypeWithoutSuffix
                    .EndsWith("hateoas", StringComparison.InvariantCultureIgnoreCase);

                var primaryMediaType = includeLinks ?
                    parsedMediaType.SubTypeWithoutSuffix
                    .Substring(0, parsedMediaType.SubTypeWithoutSuffix.Length - 8)
                    : parsedMediaType.SubTypeWithoutSuffix;

                bool includeChildren = primaryMediaType.ToString().Contains("withchildren");

                Farm farmAsEntity = await FindFarm(id, userId, isAdmin, includeChildren);

                if (farmAsEntity == null) return GenericResponseBuilder.NotFound<IDictionary<string, object>>();

                IEnumerable<LinkDto> links = new List<LinkDto>();
                if (includeLinks)
                {
                    links = UrlCreatorHelper.CreateLinksForFarm(url, id, childrenResourceParameter.Fields);
                }

                if (includeChildren)
                {
                    var farmToReturnWithChildrenShaped = CreateFarmWithChildrenAsDictionary(
                        childrenResourceParameter,
                        childrenResourceParameter.PageNumber, 
                        childrenResourceParameter.PageSize,
                        includeLinks, 
                        farmAsEntity, 
                        links);

                    return GenericResponseBuilder.Success<IDictionary<string, object>>(farmToReturnWithChildrenShaped);
                }

                var farmToReturn = this.mapper.Map<FarmDto>(farmAsEntity)
                    .ShapeData(childrenResourceParameter.Fields) as IDictionary<string, object>;

                if (includeLinks)
                {
                    links = UrlCreatorHelper.CreateLinksForFarm(url, id, childrenResourceParameter.Fields);
                    farmToReturn.Add("links", links);
                }

                return GenericResponseBuilder.Success<IDictionary<string, object>>(farmToReturn);
            }
            catch (Exception ex)
            {
                logger.LogError(string.Format("Error in BLL - GetFarmDto. {0}", ex.Message), ex);
                return GenericResponseBuilder.NoSuccess<IDictionary<string, object>>(null, $"{ex.Message} InnerException: {ex.InnerException.Message}");
            }
        }       

        public async Task<GenericResponse<ShapedDataWithLinks>> GetFarms(Guid userId, FarmResourceParameter resourceParameter, string mediaType)
        {
            try
            {
                if (!MediaTypeHeaderValue.TryParse(mediaType,
                       out MediaTypeHeaderValue parsedMediaType))
                    return GenericResponseBuilder.NoSuccess<ShapedDataWithLinks>(null, "Wrong media type.");

                if (!propertyCheckerService.TypeHasProperties<FarmDto>(resourceParameter.Fields, true))
                    return GenericResponseBuilder.NoSuccess<ShapedDataWithLinks>(null, "Wrong fields entered or missing 'id' field");

                if (!propertyMappingService.ValidMappingExistsFor<FarmDto, Farm>(resourceParameter.OrderBy))
                    return GenericResponseBuilder.NoSuccess<ShapedDataWithLinks>(null, "Wrong OrderBy entered");

                var includeLinks = parsedMediaType.SubTypeWithoutSuffix
                    .EndsWith("hateoas", StringComparison.InvariantCultureIgnoreCase);

                var primaryMediaType = includeLinks ?
                    parsedMediaType.SubTypeWithoutSuffix
                    .Substring(0, parsedMediaType.SubTypeWithoutSuffix.Length - 8)
                    : parsedMediaType.SubTypeWithoutSuffix;

                bool includeChildren = primaryMediaType.ToString().Contains("withchildren");

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
                            resourceParameter.ChildPageNumber,
                            resourceParameter.ChildPageSize,
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
                return GenericResponseBuilder.NoSuccess<ShapedDataWithLinks>(null, $"{ex.Message} InnerException: {ex.InnerException.Message}");
            }
        }

        public async Task<GenericResponse> UpdateFarm(Farm farm, FarmForUpdateDto farmToPatch)
        {
            try
            {
                this.mapper.Map(farmToPatch, farm);

                this.dataService.Farms.Update(farm);
                await this.dataService.CompleteAsync();

                return GenericResponseBuilder.Success();
            }
            catch (Exception ex)
            {
                logger.LogError(string.Format("Error in BLL - UpdateFarm. {0}", ex.Message), ex);
                return GenericResponseBuilder.NoSuccess($"{ex.Message} InnerException: {ex.InnerException.Message}");
            }
        }

        #region Helpers
        private async Task<Farm> FindFarm(Guid id, Guid userId, object isAdmin, bool includeChildren)
        {
            Farm existingFarm = null;

            if (isAdmin == null)
            {
                existingFarm = await this.dataService
                .Farms
                .FindByCondition(
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

        private IDictionary<string, object> CreateFarmWithChildrenAsDictionary(BaseResourceParameter childrenResourceParameter, int pageNumber, int pageSize, bool includeLinks, Farm farmAsEntity, IEnumerable<LinkDto> links)
        {
            try
            {
                var fieldsToReturn = ShapeFieldsAsChildren(
                farmAsEntity,
                pageNumber,
                pageSize,
                childrenResourceParameter,
                includeLinks);

                var farmToReturnWithChildren = this.mapper.Map<FarmWithShapedChildrenDto>(farmAsEntity);
                farmToReturnWithChildren.FieldsDto = fieldsToReturn;

                var farmToReturnWithChildrenShaped = farmToReturnWithChildren.ShapeData("") as IDictionary<string, object>;

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