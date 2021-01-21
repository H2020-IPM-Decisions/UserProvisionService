using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using H2020.IPMDecisions.UPR.BLL.Helpers;
using H2020.IPMDecisions.UPR.Core.Dtos;
using H2020.IPMDecisions.UPR.Core.Entities;
using H2020.IPMDecisions.UPR.Core.Helpers;
using H2020.IPMDecisions.UPR.Core.Models;
using H2020.IPMDecisions.UPR.Core.ResourceParameters;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace H2020.IPMDecisions.UPR.BLL
{
    public partial class BusinessLogic : IBusinessLogic
    {
        public async Task<GenericResponse<FieldObservationDto>> AddNewFieldObservation(FieldObservationForCreationDto fieldObservationForCreationDto, HttpContext httpContext, string mediaType)
        {
            try
            {
                var field = httpContext.Items["field"] as Field;
                var fieldCropPestExist = field
                    .FieldCrop
                    .FieldCropPests
                    .Where(fcp => fcp.Id == fieldObservationForCreationDto.FieldCropPestId)
                    .FirstOrDefault();

                if (fieldCropPestExist == null) return GenericResponseBuilder.NotFound<FieldObservationDto>();

                var observationAsEntity = this.mapper.Map<FieldObservation>(fieldObservationForCreationDto);
                observationAsEntity.FieldCropPest = fieldCropPestExist;

                this.dataService.FieldObservations.Create(observationAsEntity);
                await this.dataService.CompleteAsync();

                var fieldToReturn = this.mapper.Map<FieldObservationDto>(observationAsEntity);
                return GenericResponseBuilder.Success<FieldObservationDto>(fieldToReturn);
            }
            catch (Exception ex)
            {
                logger.LogError(string.Format("Error in BLL - AddNewFieldObservation. {0}", ex.Message), ex);
                String innerMessage = (ex.InnerException != null) ? ex.InnerException.Message : "";
                return GenericResponseBuilder.NoSuccess<FieldObservationDto>(null, $"{ex.Message} InnerException: {innerMessage}");
            }
        }

        public async Task<GenericResponse> DeleteFieldObservation(Guid id, HttpContext httpContext)
        {
            try
            {
                var field = httpContext.Items["field"] as Field;
                var existingObservation = field
                    .FieldCrop
                    .FieldCropPests
                    .SelectMany(f => f.FieldObservations)
                    .Where(fo => fo.Id == id)
                    .FirstOrDefault();

                if (existingObservation == null) return GenericResponseBuilder.Success();

                this.dataService.FieldObservations.Delete(existingObservation);
                await this.dataService.CompleteAsync();
                return GenericResponseBuilder.Success();
            }
            catch (Exception ex)
            {
                logger.LogError(string.Format("Error in BLL - DeleteFieldObservation. {0}", ex.Message), ex);
                String innerMessage = (ex.InnerException != null) ? ex.InnerException.Message : "";
                return GenericResponseBuilder.NoSuccess($"{ex.Message} InnerException: {innerMessage}");
            }
        }

        public GenericResponse<FieldObservationDto> GetFieldObservationDto(Guid id, string fields, string mediaType, HttpContext httpContext)
        {
            try
            {
                if (!MediaTypeHeaderValue.TryParse(mediaType,
                        out MediaTypeHeaderValue parsedMediaType))
                    return GenericResponseBuilder.NoSuccess<FieldObservationDto>(null, "Wrong media type.");

                if (!propertyCheckerService.TypeHasProperties<FieldObservationDto>(fields, false))
                    return GenericResponseBuilder.NoSuccess<FieldObservationDto>(null, "Wrong fields entered");

                var field = httpContext.Items["field"] as Field;
                var observationAsEntity = field
                    .FieldCrop
                    .FieldCropPests
                    .SelectMany(f => f.FieldObservations)
                    .Where(fo => fo.Id == id)
                    .FirstOrDefault();

                if (observationAsEntity == null) return GenericResponseBuilder.NotFound<FieldObservationDto>();

                // ToDo: Shape Data
                var dataToReturn = this.mapper.Map<FieldObservationDto>(observationAsEntity);
                return GenericResponseBuilder.Success<FieldObservationDto>(dataToReturn);
            }
            catch (Exception ex)
            {
                logger.LogError(string.Format("Error in BLL - GetFieldObservationDto. {0}", ex.Message), ex);
                String innerMessage = (ex.InnerException != null) ? ex.InnerException.Message : "";
                return GenericResponseBuilder.NoSuccess<FieldObservationDto>(null, $"{ex.Message} InnerException: {innerMessage}");
            }
        }

        public async Task<GenericResponse<ShapedDataWithLinks>> GetFieldObservations(Guid fieldId, FieldObservationResourceParameter resourceParameter, string mediaType)
        {
            try
            {
                if (!MediaTypeHeaderValue.TryParse(mediaType,
                      out MediaTypeHeaderValue parsedMediaType))
                    return GenericResponseBuilder.NoSuccess<ShapedDataWithLinks>(null, "Wrong media type.");

                if (!propertyCheckerService.TypeHasProperties<FieldObservationDto>(resourceParameter.Fields, true))
                    return GenericResponseBuilder.NoSuccess<ShapedDataWithLinks>(null, "Wrong fields entered or missing 'id' field");

                if (!propertyMappingService.ValidMappingExistsFor<FieldObservationDto, FieldObservation>(resourceParameter.OrderBy))
                    return GenericResponseBuilder.NoSuccess<ShapedDataWithLinks>(null, "Wrong OrderBy entered");

                var observationsAsEntities = await this
                    .dataService
                    .FieldObservations
                    .FindAllAsync(resourceParameter, fieldId);

                if (observationsAsEntities.Count == 0) return GenericResponseBuilder.NotFound<ShapedDataWithLinks>();

                var paginationMetaData = MiscellaneousHelper.CreatePaginationMetadata(observationsAsEntities);

                var links = UrlCreatorHelper.CreateLinksForFieldObservations(
                    this.url,
                    fieldId,
                    resourceParameter,
                    observationsAsEntities.HasNext,
                    observationsAsEntities.HasPrevious);

                var shapedObservationsToReturn = this.mapper
                    .Map<IEnumerable<FieldObservationDto>>(observationsAsEntities)
                    .ShapeData(resourceParameter.Fields);

                var dataToReturn = new ShapedDataWithLinks()
                {
                    Value = shapedObservationsToReturn,
                    Links = links,
                    PaginationMetaData = paginationMetaData
                };

                return GenericResponseBuilder.Success<ShapedDataWithLinks>(dataToReturn);
            }
            catch (Exception ex)
            {
                logger.LogError(string.Format("Error in BLL - GetFieldObservations. {0}", ex.Message), ex);
                String innerMessage = (ex.InnerException != null) ? ex.InnerException.Message : "";
                return GenericResponseBuilder.NoSuccess<ShapedDataWithLinks>(null, $"{ex.Message} InnerException: {innerMessage}");
            }
        }

        #region Helpers
        private ShapedDataWithLinks ShapeFieldObservationsAsChildren(FieldCrop fieldCrop, Guid fieldCropPestId, FieldObservationResourceParameter resourceParameter, bool includeLinks)
        {
            try
            {
                var childrenAsPaged = PagedList<FieldObservation>.Create(
                    fieldCrop.FieldCropPests.Where(f => f.Id == fieldCropPestId).SelectMany(f => f.FieldObservations).AsQueryable(),
                    resourceParameter.PageNumber,
                    resourceParameter.PageSize);

                var childrenPaginationLinks = UrlCreatorHelper.CreateLinksForFieldObservations(
                    this.url,
                    fieldCrop.FieldId,
                    resourceParameter,
                    childrenAsPaged.HasNext,
                    childrenAsPaged.HasPrevious);

                var paginationMetaDataChildren = MiscellaneousHelper.CreatePaginationMetadata(childrenAsPaged);

                var shapedChildrenToReturn = this.mapper
                    .Map<IEnumerable<FieldObservationDto>>(childrenAsPaged)
                    .ShapeData(resourceParameter.Fields);

                var shapedChildrenToReturnWithLinks = shapedChildrenToReturn.Select(fieldObservation =>
                {
                    var fieldObservationAsDictionary = fieldObservation as IDictionary<string, object>;
                    if (includeLinks)
                    {
                        var userLinks = UrlCreatorHelper.CreateLinksForFieldObservation(
                            this.url,
                            (Guid)fieldObservationAsDictionary["Id"],
                            fieldCrop.FieldId,
                            resourceParameter.Fields);
                        fieldObservationAsDictionary.Add("links", userLinks);
                    }
                    return fieldObservationAsDictionary;
                });

                return new ShapedDataWithLinks()
                {
                    Value = shapedChildrenToReturnWithLinks,
                    Links = childrenPaginationLinks,
                    PaginationMetaData = paginationMetaDataChildren
                };
            }
            catch (Exception ex)
            {
                logger.LogError(string.Format("Error in BLL - ShapeFieldObservationsAsChildren. {0}", ex.Message), ex);
                return null;
            }
        }
        #endregion
    }
}