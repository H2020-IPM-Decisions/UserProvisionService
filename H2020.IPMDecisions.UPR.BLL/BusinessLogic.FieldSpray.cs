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
        public async Task<GenericResponse<FieldSprayApplicationDto>> AddNewFieldSpray(FieldSprayApplicationForCreationDto sprayForCreationDto, HttpContext httpContext, string mediaType)
        {
            try
            {
                var field = httpContext.Items["field"] as Field;
                var fieldCropPestExist = field
                    .FieldCrop
                    .FieldCropPests
                    .Where(fcp => fcp.Id == sprayForCreationDto.FieldCropPestId)
                    .FirstOrDefault();

                if (fieldCropPestExist == null) return GenericResponseBuilder.NotFound<FieldSprayApplicationDto>();

                var objectAsEntity = this.mapper.Map<FieldSprayApplication>(sprayForCreationDto);
                objectAsEntity.FieldCropPest = fieldCropPestExist;

                this.dataService.FieldSprayApplication.Create(objectAsEntity);
                await this.dataService.CompleteAsync();

                var fieldToReturn = this.mapper.Map<FieldSprayApplicationDto>(objectAsEntity);
                return GenericResponseBuilder.Success(fieldToReturn);
            }
            catch (Exception ex)
            {
                logger.LogError(string.Format("Error in BLL - AddNewFieldSpray. {0}", ex.Message), ex);
                String innerMessage = (ex.InnerException != null) ? ex.InnerException.Message : "";
                return GenericResponseBuilder.NoSuccess<FieldSprayApplicationDto>(null, $"{ex.Message} InnerException: {innerMessage}");
            }
        }

        public async Task<GenericResponse> DeleteFieldSpray(Guid id, HttpContext httpContext)
        {
            try
            {
                var field = httpContext.Items["field"] as Field;
                var existingEntity = field
                    .FieldCrop
                    .FieldCropPests
                    .SelectMany(f => f.FieldSprayApplications)
                    .Where(fs => fs.Id == id)
                    .FirstOrDefault();

                if (existingEntity == null) return GenericResponseBuilder.Success();

                this.dataService.FieldSprayApplication.Delete(existingEntity);
                await this.dataService.CompleteAsync();
                return GenericResponseBuilder.Success();

            }
            catch (Exception ex)
            {
                logger.LogError(string.Format("Error in BLL - DeleteFieldSpray. {0}", ex.Message), ex);
                String innerMessage = (ex.InnerException != null) ? ex.InnerException.Message : "";
                return GenericResponseBuilder.NoSuccess($"{ex.Message} InnerException: {innerMessage}");
            }
        }

        public GenericResponse<FieldSprayApplicationDto> GetFieldSprayDto(Guid id, string mediaType, HttpContext httpContext)
        {
            try
            {
                if (!MediaTypeHeaderValue.TryParse(mediaType,
                        out MediaTypeHeaderValue parsedMediaType))
                    return GenericResponseBuilder.NoSuccess<FieldSprayApplicationDto>(null, "Wrong media type.");

                var field = httpContext.Items["field"] as Field;
                var sprayAsEntity = field
                    .FieldCrop
                    .FieldCropPests
                    .SelectMany(f => f.FieldSprayApplications)
                    .Where(fs => fs.Id == id)
                    .FirstOrDefault();

                if (sprayAsEntity == null) return GenericResponseBuilder.NotFound<FieldSprayApplicationDto>();

                // ToDo: Shape Data
                var dataToReturn = this.mapper.Map<FieldSprayApplicationDto>(sprayAsEntity);
                return GenericResponseBuilder.Success<FieldSprayApplicationDto>(dataToReturn);

            }
            catch (Exception ex)
            {
                logger.LogError(string.Format("Error in BLL - GetFieldSprayDto. {0}", ex.Message), ex);
                String innerMessage = (ex.InnerException != null) ? ex.InnerException.Message : "";
                return GenericResponseBuilder.NoSuccess<FieldSprayApplicationDto>(null, $"{ex.Message} InnerException: {innerMessage}");
            }
        }

        public async Task<GenericResponse<ShapedDataWithLinks>> GetFieldSprays(Guid fieldId, FieldSprayResourceParameter resourceParameter, string mediaType)
        {
            try
            {
                if (!MediaTypeHeaderValue.TryParse(mediaType,
                      out MediaTypeHeaderValue parsedMediaType))
                    return GenericResponseBuilder.NoSuccess<ShapedDataWithLinks>(null, "Wrong media type.");

                if (!propertyCheckerService.TypeHasProperties<FieldSprayApplicationDto>(resourceParameter.Fields, true))
                    return GenericResponseBuilder.NoSuccess<ShapedDataWithLinks>(null, "Wrong fields entered or missing 'id' field");

                var dataAsEntities = await this
                    .dataService
                    .FieldSprayApplication
                    .FindAllAsync(resourceParameter, fieldId);

                var paginationMetaData = MiscellaneousHelper.CreatePaginationMetadata(dataAsEntities);
                var links = UrlCreatorHelper.CreateLinksForFieldSprays(
                    this.url,
                    fieldId,
                    resourceParameter,
                    dataAsEntities.HasNext,
                    dataAsEntities.HasPrevious);

                var shapedDataToReturn = this.mapper
                    .Map<IEnumerable<FieldSprayApplicationDto>>(dataAsEntities)
                    .ShapeData(resourceParameter.Fields);

                var dataToReturn = new ShapedDataWithLinks()
                {
                    Value = shapedDataToReturn,
                    Links = links,
                    PaginationMetaData = paginationMetaData
                };

                return GenericResponseBuilder.Success<ShapedDataWithLinks>(dataToReturn);
            }
            catch (Exception ex)
            {
                logger.LogError(string.Format("Error in BLL - GetFieldSprayApplications. {0}", ex.Message), ex);
                String innerMessage = (ex.InnerException != null) ? ex.InnerException.Message : "";
                return GenericResponseBuilder.NoSuccess<ShapedDataWithLinks>(null, $"{ex.Message} InnerException: {innerMessage}");
            }
        }

        #region Helpers
        private ShapedDataWithLinks ShapeFieldSpraysAsChildren(FieldCrop fieldCrop, Guid fieldCropPestId, FieldSprayResourceParameter resourceParameter, bool includeLinks)
        {
            try
            {
                var childrenAsPaged = PagedList<FieldSprayApplication>.Create(
                    fieldCrop.FieldCropPests.Where(f => f.Id == fieldCropPestId).SelectMany(f => f.FieldSprayApplications).AsQueryable(),
                    resourceParameter.PageNumber,
                    resourceParameter.PageSize);

                resourceParameter.FieldCropPestId = fieldCropPestId;
                var childrenPaginationLinks = UrlCreatorHelper.CreateLinksForFieldSprays(
                    this.url,
                    fieldCrop.FieldId,
                    resourceParameter,
                    childrenAsPaged.HasNext,
                    childrenAsPaged.HasPrevious);

                var paginationMetaDataChildren = MiscellaneousHelper.CreatePaginationMetadata(childrenAsPaged);

                var shapedChildrenToReturn = this.mapper
                    .Map<IEnumerable<FieldSprayApplicationDto>>(childrenAsPaged)
                    .ShapeData(resourceParameter.Fields);

                var shapedChildrenToReturnWithLinks = shapedChildrenToReturn.Select(fieldSpray =>
                {
                    var fieldSprayAsDictionary = fieldSpray as IDictionary<string, object>;
                    if (includeLinks)
                    {
                        var userLinks = UrlCreatorHelper.CreateLinksForFieldSpray(
                            this.url,
                            (Guid)fieldSprayAsDictionary["Id"],
                            fieldCrop.FieldId,
                            resourceParameter.Fields);
                        fieldSprayAsDictionary.Add("links", userLinks);
                    }
                    return fieldSprayAsDictionary;
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