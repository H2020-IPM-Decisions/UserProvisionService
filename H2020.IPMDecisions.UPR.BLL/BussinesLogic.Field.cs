using H2020.IPMDecisions.UPR.BLL.Helpers;
using H2020.IPMDecisions.UPR.Core.Dtos;
using H2020.IPMDecisions.UPR.Core.Entities;
using H2020.IPMDecisions.UPR.Core.Helpers;
using H2020.IPMDecisions.UPR.Core.Models;
using H2020.IPMDecisions.UPR.Core.ResourceParameters;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
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

                var links = UrlCreatorHelper.CreateLinksForFields(
                    this.url,
                    farmId,
                    resourceParameter,
                    fieldsAsEntities.HasNext,
                    fieldsAsEntities.HasPrevious);

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
        private ShapedDataWithLinks ShapeFieldsAsChildren(Farm farm, int pageNumer, int pageSize, BaseResourceParameter resourceParameter, bool includeLinks)
        {
            try
            {
                var childrenAsPaged = PagedList<Field>.Create(
                farm.Fields.AsQueryable(),
                pageNumer,
                pageSize);

                var fieldResourceParameter = this.mapper.Map<FieldResourceParameter>(resourceParameter);
                var childrenPaginationLinks = UrlCreatorHelper.CreateLinksForFields(
                    this.url,
                    farm.Id,
                    fieldResourceParameter,
                    childrenAsPaged.HasNext,
                    childrenAsPaged.HasPrevious);

                var paginationMetaDataChildren = MiscellaneousHelper.CreatePaginationMetadata(childrenAsPaged);

                var shapedChildrenToReturn = this.mapper
                    .Map<IEnumerable<FieldDto>>(childrenAsPaged)
                    .ShapeData("");

                var shapedChildrentToReturnWithLinks = shapedChildrenToReturn.Select(field =>
                {
                    var farmAsDictionary = field as IDictionary<string, object>;
                    if (includeLinks)
                    {
                        var userLinks = UrlCreatorHelper.CreateLinksForField(
                            this.url,
                            (Guid)farmAsDictionary["Id"],
                            farm.Id,
                            fieldResourceParameter.Fields);
                        farmAsDictionary.Add("links", userLinks);
                    }
                    return farmAsDictionary;
                });

                return new ShapedDataWithLinks()
                {
                    Value = shapedChildrentToReturnWithLinks,
                    Links = childrenPaginationLinks,
                    PaginationMetaData = paginationMetaDataChildren
                };

            }
            catch (Exception ex)
            {
                throw ex;
                //ToDo Log Error
                // Log($"{ex.Message} InnerException: {ex.InnerException.Message}");
            }
        }
        #endregion
    }
}