using H2020.IPMDecisions.UPR.Core.Dtos;
using H2020.IPMDecisions.UPR.Core.Entities;
using H2020.IPMDecisions.UPR.Core.Helpers;
using H2020.IPMDecisions.UPR.Core.Models;
using H2020.IPMDecisions.UPR.Core.ResourceParameters;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Threading.Tasks;

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

        public async Task<GenericResponse<FieldDto>> AddNewField(FieldForCreationDto fieldForCreationDto, HttpContext httpContext, Guid id)
        {
            try
            {
                var farm = httpContext.Items["farm"] as Farm;

                var fieldAsEntity = this.mapper.Map<Field>(fieldForCreationDto);
                fieldAsEntity.Farm = farm;
                fieldAsEntity.Id = id;

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

                var links = CreateLinksForFields(farmId, resourceParameter, fieldsAsEntities.HasNext, fieldsAsEntities.HasPrevious);

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

        public async Task<GenericResponse<Field>> GetField(Guid id)
        {
            try
            {
                var fieldAsEntity = await this
                    .dataService
                    .Fields
                    .FindByIdAsync(id);

                if (fieldAsEntity == null) return GenericResponseBuilder.Success<Field>(null);

                return GenericResponseBuilder.Success<Field>(fieldAsEntity);
            }
            catch (Exception ex)
            {
                //ToDo Log Error
                return GenericResponseBuilder.NoSuccess<Field>(null, $"{ex.Message} InnerException: {ex.InnerException.Message}");
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

        public async Task<GenericResponse> UpdateField(Field field, FieldForUpdateDto fieldToPatch)
        {
            try
            {
                this.mapper.Map(fieldToPatch, field);

                this.dataService.Fields.Update(field);
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
        public FieldForCreationDto MapToFieldForCreation(FieldForUpdateDto fieldForUpdateDto)
        {
            return this.mapper.Map<FieldForCreationDto>(fieldForUpdateDto);
        }
        public FieldForUpdateDto MapToFieldForUpdateDto(Field field)
        {
            return this.mapper.Map<FieldForUpdateDto>(field);
        }

        private IEnumerable<LinkDto> CreateLinksForFields(
            Guid farmId,
            FieldResourceParameter resourceParameter,
            bool hasNextPage,
            bool hasPreviousPage)
        {
            var links = new List<LinkDto>();

            links.Add(new LinkDto(
                CreateFieldResourceUri(farmId, resourceParameter, ResourceUriType.Current),
                "self",
                "GET"));

            if (hasNextPage)
            {
                links.Add(new LinkDto(
                CreateFieldResourceUri(farmId, resourceParameter, ResourceUriType.NextPage),
                "next_page",
                "GET"));
            }
            if (hasPreviousPage)
            {
                links.Add(new LinkDto(
               CreateFieldResourceUri(farmId, resourceParameter, ResourceUriType.PreviousPage),
               "previous_page",
               "GET"));
            }
            return links;
        }

        private string CreateFieldResourceUri(
            Guid farmId,
            FieldResourceParameter resourceParameter,
            ResourceUriType type)
        {
            switch (type)
            {
                case ResourceUriType.PreviousPage:
                    return url.Link("GetFields",
                    new
                    {
                        farmId,
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
                        farmId,
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
                        farmId,
                        fields = resourceParameter.Fields,
                        orderBy = resourceParameter.OrderBy,
                        pageNumber = resourceParameter.PageNumber,
                        pageSize = resourceParameter.PageSize,
                        searchQuery = resourceParameter.SearchQuery
                    });
            }
        }

        private IEnumerable<LinkDto> CreateLinksForField(
            Guid id,
            Guid farmId,
            string fields = "")
        {
            var links = new List<LinkDto>();

            if (string.IsNullOrWhiteSpace(fields))
            {
                links.Add(new LinkDto(
                url.Link("GetFieldById", new { farmId, id }),
                "self",
                "GET"));
            }
            else
            {
                links.Add(new LinkDto(
                 url.Link("GetFieldById", new { farmId, id, fields }),
                 "self",
                 "GET"));
            }

            links.Add(new LinkDto(
                url.Link("DeleteField", new { farmId, id }),
                "delete_field",
                "DELETE"));

            links.Add(new LinkDto(
                url.Link("PartialUpdateField", new { farmId, id }),
                "update_field",
                "PATCH"));

            return links;
        }
        #endregion
    }
}