using System;
using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using H2020.IPMDecisions.UPR.Core.Dtos;
using H2020.IPMDecisions.UPR.Core.Entities;
using H2020.IPMDecisions.UPR.Core.Helpers;
using H2020.IPMDecisions.UPR.Core.Models;
using H2020.IPMDecisions.UPR.Core.ResourceParameters;
using Microsoft.AspNetCore.Http;

namespace H2020.IPMDecisions.UPR.BLL
{
    public partial class BusinessLogic : IBusinessLogic
    {
        public async Task<GenericResponse<FieldDto>> AddNewField(
            FieldForCreationDto fieldForCreationDto,
            HttpContext httpContext, 
            string mediaType)
        {
            try
            {
                var userId = Guid.Parse(httpContext.Items["userId"].ToString());
                var farm = httpContext.Items["farm"] as Farm;

                var fieldAsEntity = this.mapper.Map<Field>(fieldForCreationDto);
                fieldAsEntity.Farm = farm;

                this.dataService.Fields.Create(fieldAsEntity);
                await this.dataService.CompleteAsync();

                var fieldToReturn = this.mapper.Map<FieldDto>(fieldAsEntity);
                return GenericResponseBuilder.Success<FieldDto>(fieldToReturn);
            }
            catch (Exception ex)
            {
                //ToDo Log Error
                return GenericResponseBuilder.NoSuccess<FieldDto>(null, $"{ex.Message} InnerException: {ex.InnerException.Message}");
            }
        }

        public async Task<GenericResponse> DeleteField(Guid id)
        {
            try
            {
                var existingField = await this
                            .dataService
                            .Fields
                            .FindByIdAsync(id);

                if (existingField == null) return GenericResponseBuilder.Success();

                this.dataService.Fields.Delete(existingField);
                await this.dataService.CompleteAsync();

                return GenericResponseBuilder.Success();
            }
            catch (Exception ex)
            {
                //ToDo Log Error
                return GenericResponseBuilder.NoSuccess($"{ex.Message} InnerException: {ex.InnerException.Message}");
            }
        }

        public async Task<GenericResponse<ShapedDataWithLinks>> GetFields(
            Guid farmId, 
            FieldResourceParameter resourceParameter, 
            string mediaType)
        {
            try
            {
                if (!MediaTypeHeaderValue.TryParse(mediaType,
                       out MediaTypeHeaderValue parsedMediaType))
                    return GenericResponseBuilder.NoSuccess<ShapedDataWithLinks>(null, "Wrong media type.");

                if (!propertyCheckerService.TypeHasProperties<FarmDto>(resourceParameter.Fields, true))
                    return GenericResponseBuilder.NoSuccess<ShapedDataWithLinks>(null, "Wrong fields entered or missing 'id' field");

                if (!propertyMappingService.ValidMappingExistsFor<FieldDto, Field>(resourceParameter.OrderBy))
                    return GenericResponseBuilder.NoSuccess<ShapedDataWithLinks>(null, "Wrong OrderBy entered");

                var fieldsAsEntities = await this
                    .dataService
                    .Fields
                    .FindAllAsync(resourceParameter, farmId);

                if (fieldsAsEntities.Count == 0) return GenericResponseBuilder.NotFound<ShapedDataWithLinks>();

                var paginationMetaData = new PaginationMetaData()
                {
                    TotalCount = fieldsAsEntities.TotalCount,
                    PageSize = fieldsAsEntities.PageSize,
                    CurrentPage = fieldsAsEntities.CurrentPage,
                    TotalPages = fieldsAsEntities.TotalPages
                };

                var links = CreateLinksForFields(resourceParameter, fieldsAsEntities.HasNext, fieldsAsEntities.HasPrevious);

                var shapedFarmsToReturn = this.mapper
                    .Map<IEnumerable<FieldDto>>(fieldsAsEntities)
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

        public async Task<GenericResponse<FieldDto>> GetFieldDto(Guid id, string fields, string mediaType)
        {
            try
            {
                if (!MediaTypeHeaderValue.TryParse(mediaType,
                        out MediaTypeHeaderValue parsedMediaType))
                    return GenericResponseBuilder.NoSuccess<FieldDto>(null, "Wrong media type.");

                if (!propertyCheckerService.TypeHasProperties<FieldDto>(fields, false))
                    return GenericResponseBuilder.NoSuccess<FieldDto>(null, "Wrong fields entered");


                var fieldAsEntity = await this
                            .dataService
                            .Fields
                            .FindByIdAsync(id);

                if (fieldAsEntity == null) return GenericResponseBuilder.NotFound<FieldDto>();

                // ToDo: Shape Data

                var fieldToReturn = this.mapper.Map<FieldDto>(fieldAsEntity);
                return GenericResponseBuilder.Success<FieldDto>(fieldToReturn);
            }
            catch (Exception ex)
            {
                //ToDo Log Error
                return GenericResponseBuilder.NoSuccess<FieldDto>(null, $"{ex.Message} InnerException: {ex.InnerException.Message}");
            }
        }


        #region Helpers
        private IEnumerable<LinkDto> CreateLinksForFields(
            FieldResourceParameter resourceParameter,
            bool hasNextPage,
            bool hasPreviousPage)
        {
            var links = new List<LinkDto>();

            links.Add(new LinkDto(
                CreateFieldResourceUri(resourceParameter, ResourceUriType.Current),
                "self",
                "GET"));

            if (hasNextPage)
            {
                links.Add(new LinkDto(
                CreateFieldResourceUri(resourceParameter, ResourceUriType.NextPage),
                "next_page",
                "GET"));
            }
            if (hasPreviousPage)
            {
                links.Add(new LinkDto(
               CreateFieldResourceUri(resourceParameter, ResourceUriType.PreviousPage),
               "previous_page",
               "GET"));
            }
            return links;
        }

        private string CreateFieldResourceUri(
            FieldResourceParameter resourceParameter,
            ResourceUriType type)
        {
            switch (type)
            {
                case ResourceUriType.PreviousPage:
                    return url.Link("GetFields",
                    new
                    {
                        fields = resourceParameter.Fields,
                        orderBy = resourceParameter.OrderBy,
                        pageNumber = resourceParameter.PageNumber - 1,
                        pageSize = resourceParameter.PageSize,
                        searchQuery = resourceParameter.SearchQuery
                    });
                case ResourceUriType.NextPage:
                    return url.Link("GetFields",
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
                    return url.Link("GetFields",
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