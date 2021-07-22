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
        public async Task<GenericResponse<IDictionary<string, object>>> AddNewFieldCropDecision(
            CropPestDssForCreationDto cropPestDssForCreationDto,
            HttpContext httpContext,
            string mediaType)
        {
            try
            {
                var field = httpContext.Items["field"] as Field;
                var duplicatedRecord = field
                    .FieldCrop
                    .FieldCropPests
                    .Any(f => f.FieldCropPestDsses
                        .Any(fcpd =>
                            fcpd.FieldCropPestId == cropPestDssForCreationDto.FieldCropPestId
                            & fcpd.CropPestDss.DssId == cropPestDssForCreationDto.DssId
                            & fcpd.CropPestDss.DssModelId == cropPestDssForCreationDto.DssModelId));
                if (duplicatedRecord)
                    return GenericResponseBuilder.Duplicated<IDictionary<string, object>>();

                var getFieldCropPest = field
                    .FieldCrop
                    .FieldCropPests
                    .Where(f => f.Id == cropPestDssForCreationDto.FieldCropPestId)
                    .FirstOrDefault();
                if (getFieldCropPest == null)
                    return GenericResponseBuilder.NotFound<IDictionary<string, object>>();

                var cropPestDss = this.mapper.Map<CropPestDss>(cropPestDssForCreationDto);
                var newFieldCropPestDss = await CreateFieldCropPestDss(
                    getFieldCropPest,
                    cropPestDss,
                    cropPestDssForCreationDto.DssParameters);
                await this.dataService.CompleteAsync();

                var fieldCropPestToReturn = this.mapper
                    .Map<FieldCropPestDssDto>(newFieldCropPestDss)
                    .ShapeData() as IDictionary<string, object>;

                if (cropPestDssForCreationDto.DssExecutionType.ToLower() == "onthefly")
                {
                    var jobId = this.queueJobs.AddDssOnOnTheFlyQueue(newFieldCropPestDss.Id);
                    return GenericResponseBuilder.Accepted<IDictionary<string, object>>(fieldCropPestToReturn);
                }

                return GenericResponseBuilder.Success<IDictionary<string, object>>(fieldCropPestToReturn);
            }
            catch (Exception ex)
            {
                logger.LogError(string.Format("Error in BLL - AddNewFieldCropDecision. {0}", ex.Message), ex);
                String innerMessage = (ex.InnerException != null) ? ex.InnerException.Message : "";
                return GenericResponseBuilder.NoSuccess<IDictionary<string, object>>(null, $"{ex.Message} InnerException: {innerMessage}");
            }
        }

        public async Task<GenericResponse> DeleteFieldCropDecision(Guid id, HttpContext httpContext)
        {
            try
            {
                var field = httpContext.Items["field"] as Field;

                var fieldCropPestExist = field
                    .FieldCrop
                    .FieldCropPests
                    .SelectMany(f => f.FieldCropPestDsses)
                    .Where(fcp => fcp.Id == id)
                    .FirstOrDefault();
                if (fieldCropPestExist == null) return GenericResponseBuilder.Success();

                this.dataService.FieldCropPestDsses.Delete(fieldCropPestExist);
                await this.dataService.CompleteAsync();
                return GenericResponseBuilder.Success();
            }
            catch (Exception ex)
            {
                logger.LogError(string.Format("Error in BLL - DeleteFieldCropDecision. {0}", ex.Message), ex);
                String innerMessage = (ex.InnerException != null) ? ex.InnerException.Message : "";
                return GenericResponseBuilder.NoSuccess($"{ex.Message} InnerException: {innerMessage}");
            }
        }

        public GenericResponse<IDictionary<string, object>> GetFieldCropDecision(Guid id, HttpContext httpContext, string mediaType)
        {
            try
            {
                var field = httpContext.Items["field"] as Field;
                var fieldCropDssExist = field
                    .FieldCrop
                    .FieldCropPests
                    .SelectMany(f => f.FieldCropPestDsses)
                    .Where(x => x.Id == id)
                    .FirstOrDefault();
                if (fieldCropDssExist == null) return GenericResponseBuilder.NotFound<IDictionary<string, object>>();

                var fieldCropDssToReturn = this.mapper
                    .Map<FieldCropPestDssDto>(fieldCropDssExist)
                    .ShapeData() as IDictionary<string, object>;
                return GenericResponseBuilder.Success<IDictionary<string, object>>(fieldCropDssToReturn);
            }
            catch (Exception ex)
            {
                logger.LogError(string.Format("Error in BLL - GetFieldCropDecision. {0}", ex.Message), ex);
                String innerMessage = (ex.InnerException != null) ? ex.InnerException.Message : "";
                return GenericResponseBuilder.NoSuccess<IDictionary<string, object>>(null, $"{ex.Message} InnerException: {innerMessage}");
            }
        }

        public async Task<GenericResponse<ShapedDataWithLinks>> GetFieldCropDecisions(FieldCropPestDssResourceParameter resourceParameter, HttpContext httpContext, string mediaType)
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

                var field = httpContext.Items["field"] as Field;

                var fieldCropDssExist = field
                    .FieldCrop
                    .FieldCropPests
                    .Where(f => f.Id == resourceParameter.FieldCropPestId)
                    .Select(f => f.FieldCropPestDsses)
                    .ToList();

                if (fieldCropDssExist.Count == 0) return GenericResponseBuilder.NotFound<ShapedDataWithLinks>();

                var fieldCropDssAsEntities = await this
                    .dataService
                    .FieldCropPestDsses
                    .FindAllAsync(resourceParameter, true);

                var paginationMetaData = MiscellaneousHelper.CreatePaginationMetadata(fieldCropDssAsEntities);
                var links = UrlCreatorHelper.CreateLinksForFieldCropDecisions(
                        this.url,
                        field.Id,
                        resourceParameter,
                        fieldCropDssAsEntities.HasNext,
                        fieldCropDssAsEntities.HasPrevious);

                var shapedObservationsToReturn = this.mapper
                    .Map<IEnumerable<FieldCropPestDssDto>>(fieldCropDssAsEntities)
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
                logger.LogError(string.Format("Error in BLL - GetFieldCropDecisions. {0}", ex.Message), ex);
                String innerMessage = (ex.InnerException != null) ? ex.InnerException.Message : "";
                return GenericResponseBuilder.NoSuccess<ShapedDataWithLinks>(null, $"{ex.Message} InnerException: {innerMessage}");
            }
        }

        #region Helpers
        private async Task<FieldCropPestDss> CreateFieldCropPestDss(FieldCropPest fieldCropPest, CropPestDss cropPestDss, string dssParameters = "")
        {
            var cropPestDssExist = await this.dataService
                .CropPestDsses
                .FindByConditionAsync(c =>
                c.CropPestId == fieldCropPest.CropPest.Id
                & c.DssId == cropPestDss.DssId
                & c.DssModelId == cropPestDss.DssModelId
                & c.DssModelVersion == cropPestDss.DssModelVersion
                & c.DssExecutionType.ToLower() == cropPestDss.DssExecutionType.ToLower());

            if (cropPestDssExist == null)
            {
                cropPestDssExist = cropPestDss;
                cropPestDssExist.CropPest = fieldCropPest.CropPest;
                this.dataService.CropPestDsses.Create(cropPestDssExist);
            }

            var newFieldCropPestDss = new FieldCropPestDss()
            {
                FieldCropPest = fieldCropPest,
                CropPestDssId = cropPestDssExist.Id,
                DssParameters = dssParameters
            };
            this.dataService.FieldCropPestDsses.Create(newFieldCropPestDss);
            return newFieldCropPestDss;
        }

        private ShapedDataWithLinks ShapeFieldCropPestDssAsChildren(FieldCrop fieldCrop, Guid fieldCropPestId, FieldCropPestDssResourceParameter resourceParameter, bool includeLinks)
        {
            try
            {
                var childrenAsPaged = PagedList<FieldCropPestDss>.Create(
                    fieldCrop.FieldCropPests.Where(f => f.Id == fieldCropPestId).SelectMany(f => f.FieldCropPestDsses).AsQueryable(),
                    resourceParameter.PageNumber,
                    resourceParameter.PageSize);

                resourceParameter.FieldCropPestId = fieldCropPestId;
                var childrenPaginationLinks = UrlCreatorHelper.CreateLinksForFieldCropDecisions(
                    this.url,
                    fieldCrop.FieldId,
                    resourceParameter,
                    childrenAsPaged.HasNext,
                    childrenAsPaged.HasPrevious);

                var paginationMetaDataChildren = MiscellaneousHelper.CreatePaginationMetadata(childrenAsPaged);

                var dataAsDto = this.mapper
                    .Map<IEnumerable<FieldCropPestDssDto>>(childrenAsPaged);
                var shapedChildrenToReturn = dataAsDto.ShapeData(resourceParameter.Fields);

                var shapedChildrenToReturnWithLinks = shapedChildrenToReturn.Select(fieldCropPestDss =>
                {
                    var fieldcropPestDssAsDictionary = fieldCropPestDss as IDictionary<string, object>;
                    if (includeLinks)
                    {
                        var userLinks = UrlCreatorHelper.CreateLinksForFieldCropDecision(
                            this.url,
                            (Guid)fieldcropPestDssAsDictionary["Id"],
                            fieldCrop.FieldId,
                            resourceParameter.Fields);
                        fieldcropPestDssAsDictionary.Add("links", userLinks);
                    }
                    return fieldcropPestDssAsDictionary;
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
                logger.LogError(string.Format("Error in BLL - ShapeFieldCropPestDssAsChildren. {0}", ex.Message), ex);
                return null;
            }
        }
        #endregion
    }
}