using System;
using Microsoft.Net.Http.Headers;
using System.Threading.Tasks;
using H2020.IPMDecisions.UPR.Core.Dtos;
using H2020.IPMDecisions.UPR.Core.Entities;
using H2020.IPMDecisions.UPR.Core.Models;
using Microsoft.AspNetCore.Http;
using System.Linq;
using System.Collections.Generic;
using H2020.IPMDecisions.UPR.Core.ResourceParameters;
using H2020.IPMDecisions.UPR.Core.Helpers;

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

                Farm existingFarm = null;

                if (isAdmin == null)
                {
                    existingFarm = await this.dataService
                    .Farms
                    .FindByCondition(
                        f => f.Id == id 
                        &&
                        f.UserFarms.Any(uf => uf.UserId == userId)
                    );
                }
                else {
                    existingFarm = await this.dataService
                        .Farms
                        .FindByIdAsync(id);
                }

                if (existingFarm == null) return GenericResponseBuilder.Success();

                this.dataService.Farms.Delete(existingFarm);
                await this.dataService.CompleteAsync();
                
                return GenericResponseBuilder.Success();
            }
            catch (Exception ex)
            {
                //ToDo Log Error
                return GenericResponseBuilder.NoSuccess($"{ex.Message} InnerException: {ex.InnerException.Message}");
            }
        }

        public async Task<GenericResponse<FarmDto>> LinkNewFarmToUserProfile(FarmForCreationDto farmForCreationDto, Guid id, Guid userId)
        {
            try
            {
                var userProfile = await GetUserProfile(userId);
                if (userProfile.Result == null)
                {
                    // ToDo Create empty profile
                    return GenericResponseBuilder.NoSuccess<FarmDto>(null, "Please add User Profile first.");
                }

                var farmAsEntity = this.mapper.Map<Farm>(farmForCreationDto);
                farmAsEntity.Id = id;

                this.dataService.UserProfiles.AddFarm(userProfile.Result, farmAsEntity);
                await this.dataService.CompleteAsync();

                var farmToReturn = this.mapper.Map<FarmDto>(farmAsEntity);
                return GenericResponseBuilder.Success<FarmDto>(farmToReturn);
            }
            catch (Exception ex)
            {
                //ToDo Log Error
                return GenericResponseBuilder.NoSuccess<FarmDto>(null, $"{ex.Message} InnerException: {ex.InnerException.Message}");
            }
        }

        public async Task<GenericResponse<FarmDto>> LinkNewFarmToUserProfile(FarmForCreationDto farmForCreationDto, Guid userId, string mediaType)
        {
            try
            {
                if (!MediaTypeHeaderValue.TryParse(mediaType,
                       out MediaTypeHeaderValue parsedMediaType))
                    return GenericResponseBuilder.NoSuccess<FarmDto>(null, "Wrong media type.");

                var userProfile = await GetUserProfile(userId);
                if (userProfile.Result == null)
                {
                    // ToDo Create empty profile
                    return GenericResponseBuilder.NoSuccess<FarmDto>(null, "Please add User Profile first.");
                }
                var farmAsEntity = this.mapper.Map<Farm>(farmForCreationDto);

                this.dataService.UserProfiles.AddFarm(userProfile.Result, farmAsEntity);
                await this.dataService.CompleteAsync();

                var farmToReturn = this.mapper.Map<FarmDto>(farmAsEntity);
                return GenericResponseBuilder.Success<FarmDto>(farmToReturn);
            }
            catch (Exception ex)
            {
                //ToDo Log Error
                return GenericResponseBuilder.NoSuccess<FarmDto>(null, $"{ex.Message} InnerException: {ex.InnerException.Message}");
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
                //ToDo Log Error
                return GenericResponseBuilder.NoSuccess<Farm>(null, $"{ex.Message} InnerException: {ex.InnerException.Message}");
            }
        }

        public async Task<GenericResponse<FarmDto>> GetFarmDto(Guid id, HttpContext httpContext, string fields, string mediaType)
        {
            try
            {
                if (!MediaTypeHeaderValue.TryParse(mediaType,
                       out MediaTypeHeaderValue parsedMediaType))
                    return GenericResponseBuilder.NoSuccess<FarmDto>(null, "Wrong media type.");

                var userId = Guid.Parse(httpContext.Items["userId"].ToString());
                var isAdmin = httpContext.Items["isAdmin"];

                Farm farmAsEntity = null;

                if (isAdmin == null)
                {
                    farmAsEntity = await this
                        .dataService
                        .Farms
                        .FindByCondition(
                            f => f.Id == id &&
                            f.UserFarms
                                .Any(uf => uf.UserId == userId));
                }
                else
                {
                    farmAsEntity = await this
                        .dataService
                        .Farms
                        .FindByIdAsync(id);
                }

                if (farmAsEntity == null) return GenericResponseBuilder.NotFound<FarmDto>();

                var farmToReturn = this.mapper.Map<FarmDto>(farmAsEntity);
                return GenericResponseBuilder.Success<FarmDto>(farmToReturn);
            }
            catch (Exception ex)
            {
                //ToDo Log Error
                return GenericResponseBuilder.NoSuccess<FarmDto>(null, $"{ex.Message} InnerException: {ex.InnerException.Message}");
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

                var farmsAsEntities = await this
                    .dataService
                    .Farms
                    .FindAllAsync(resourceParameter, userId);                

                if (farmsAsEntities.Count == 0) return GenericResponseBuilder.NotFound<ShapedDataWithLinks>();

                var paginationMetaData = new PaginationMetaData()
                {
                    TotalCount = farmsAsEntities.TotalCount,
                    PageSize = farmsAsEntities.PageSize,
                    CurrentPage = farmsAsEntities.CurrentPage,
                    TotalPages = farmsAsEntities.TotalPages
                };

                var links = CreateLinksForFarms(resourceParameter, farmsAsEntities.HasNext, farmsAsEntities.HasPrevious);

                var shapedFarmsToReturn = this.mapper
                                    .Map<IEnumerable<FarmDto>>(farmsAsEntities)
                                    .ShapeData(resourceParameter.Fields);

                var farmsToReturn = new ShapedDataWithLinks()
                {
                    Value = shapedFarmsToReturn,
                    Links = links,
                    PaginationMetaData = paginationMetaData
                };

                return GenericResponseBuilder.Success<ShapedDataWithLinks>(farmsToReturn);
            }
            catch (Exception ex)
            {
                //ToDo Log Error
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
                //ToDo Log Error
                return GenericResponseBuilder.NoSuccess($"{ex.Message} InnerException: {ex.InnerException.Message}");
            }
        }

        #region Helpers

        public FarmForCreationDto MapToFarmForCreation(FarmForUpdateDto farmDto)
        {
            return this.mapper.Map<FarmForCreationDto>(farmDto);
        }

        public FarmForUpdateDto MapToFarmForUpdateDto(Farm farm)
        {
            return this.mapper.Map<FarmForUpdateDto>(farm);
        }

        private IEnumerable<LinkDto> CreateLinksForFarms(
            FarmResourceParameter resourceParameter, 
            bool hasNextPage, 
            bool hasPreviousPage)
        {
            var links = new List<LinkDto>();

            links.Add(new LinkDto(
                CreateFarmResourceUri(resourceParameter, ResourceUriType.Current),
                "self",
                "GET"));

            if (hasNextPage)
            {
                links.Add(new LinkDto(
                CreateFarmResourceUri(resourceParameter, ResourceUriType.NextPage),
                "next_page",
                "GET"));
            }
            if (hasPreviousPage)
            {
                links.Add(new LinkDto(
               CreateFarmResourceUri(resourceParameter, ResourceUriType.PreviousPage),
               "previous_page",
               "GET"));
            }
            return links;
        }

        private string CreateFarmResourceUri(
            FarmResourceParameter resourceParameter, 
            ResourceUriType type)
        {
            switch (type)
            {
                case ResourceUriType.PreviousPage:
                    return url.Link("GetFarms",
                    new
                    {
                        fields = resourceParameter.Fields,
                        orderBy = resourceParameter.OrderBy,
                        pageNumber = resourceParameter.PageNumber - 1,
                        pageSize = resourceParameter.PageSize,
                        searchQuery = resourceParameter.SearchQuery
                    });
                case ResourceUriType.NextPage:
                    return url.Link("GetFarms",
                    new
                    {
                        fields = resourceParameter.Fields,
                        orderBy = resourceParameter.OrderBy,
                        pageNumber = resourceParameter.PageNumber + 1,
                        pageSize = resourceParameter.PageSize,
                        searchQuery = resourceParameter.SearchQuery
                    });
                case ResourceUriType.Current:
                default:
                    return url.Link("GetFarms",
                    new
                    {
                        fields = resourceParameter.Fields,
                        orderBy = resourceParameter.OrderBy,
                        pageNumber = resourceParameter.PageNumber,
                        pageSize = resourceParameter.PageSize,
                        searchQuery = resourceParameter.SearchQuery
                    });
            }
        }
        #endregion
    }
}