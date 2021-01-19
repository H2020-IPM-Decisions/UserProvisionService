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
                // ToDo - FieldCrop
                // var field = httpContext.Items["field"] as Field;
                // var duplicatedRecord = field
                //     .FieldCropPests
                //     .Any(f => f.FieldCropPestDsses
                //         .Any(fcpd =>
                //             fcpd.FieldCropPestId == cropPestDssForCreationDto.FieldCropPestId
                //             & fcpd.CropPestDss.DssId == cropPestDssForCreationDto.DssId));
                // if (duplicatedRecord)
                //     return GenericResponseBuilder.Duplicated<IDictionary<string, object>>();

                // var getFieldCropPest = field
                //     .FieldCropPests
                //     .Where(f => f.Id == cropPestDssForCreationDto.FieldCropPestId)
                //     .FirstOrDefault();
                // if (getFieldCropPest == null)
                //     return GenericResponseBuilder.NotFound<IDictionary<string, object>>();

                // var newFieldCropPestDss = await CreateFieldCropPestDss(
                //     getFieldCropPest,
                //     cropPestDssForCreationDto.DssId,
                //     cropPestDssForCreationDto.DssParameters);

                // await this.dataService.CompleteAsync();

                // var fieldCropPestToReturn = this.mapper
                //     .Map<FieldCropPestDssDto>(newFieldCropPestDss)
                //     .ShapeData() as IDictionary<string, object>;
                // return GenericResponseBuilder.Success<IDictionary<string, object>>(fieldCropPestToReturn);
                throw new Exception("Database Change");

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
                // ToDo - FieldCrop
                // var field = httpContext.Items["field"] as Field;

                // var fieldCropPestExist = field
                //     .FieldCropPests
                //     .SelectMany(f => f.FieldCropPestDsses)
                //     .Where(fcp => fcp.Id == id)
                //     .FirstOrDefault();
                // if (fieldCropPestExist == null) return GenericResponseBuilder.Success();

                // this.dataService.FieldCropPestDsses.Delete(fieldCropPestExist);
                // await this.dataService.CompleteAsync();
                // return GenericResponseBuilder.Success();
                throw new Exception("Database Change");
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
                // ToDo - FieldCrop
                // var field = httpContext.Items["field"] as Field;
                // var fieldCropDssExist = field
                //     .FieldCropPests
                //     .SelectMany(f => f.FieldCropPestDsses)
                //     .Where(x => x.Id == id)
                //     .FirstOrDefault();
                // if (fieldCropDssExist == null) return GenericResponseBuilder.NotFound<IDictionary<string, object>>();

                // var fieldCropDssToReturn = this.mapper
                //     .Map<FieldCropPestDssDto>(fieldCropDssExist)
                //     .ShapeData() as IDictionary<string, object>;
                // return GenericResponseBuilder.Success<IDictionary<string, object>>(fieldCropDssToReturn);
                throw new Exception("Database Change");
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

                // ToDo - FieldCrop
                // var fieldCropDssExist = field
                //     .FieldCropPests
                //     .Where(f => f.Id == resourceParameter.FieldCropPestId)
                //     .Select(f => f.FieldCropPestDsses)
                //     .ToList();

                // if (fieldCropDssExist.Count == 0) return GenericResponseBuilder.NotFound<ShapedDataWithLinks>();

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
        private async Task<FieldCropPestDss> CreateFieldCropPestDss(FieldCropPest fieldCropPest, string dssId, string dssParameters = "")
        {
            var cropPestDssExist = await this.dataService
                .CropPestDsses
                .FindByConditionAsync(c =>
                 c.CropPestId == fieldCropPest.CropPest.Id
                 & c.DssId == dssId);

            if (cropPestDssExist == null)
            {
                cropPestDssExist = new CropPestDss()
                {
                    CropPest = fieldCropPest.CropPest,
                    DssId = dssId,
                    DssName = dssId
                };
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
        #endregion
    }
}