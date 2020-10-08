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
                    .FieldCropPests          
                    .Any(f => f.FieldCropPestDsses
                        .Any(fcpd =>
                            fcpd.FieldCropPestId == cropPestDssForCreationDto.FieldCropPestId
                            & fcpd.CropPestDss.DssId == cropPestDssForCreationDto.DssId));
                if (duplicatedRecord)
                    return GenericResponseBuilder.Duplicated<IDictionary<string, object>>();                

                var getFieldCropPest = await this.dataService
                    .FieldCropPests
                    .FindByConditionAsync(f => 
                        f.Id == cropPestDssForCreationDto.FieldCropPestId
                        & f.FieldId == field.Id);
                if (getFieldCropPest == null)
                    return GenericResponseBuilder.NotFound<IDictionary<string, object>>();

                var cropPestDssExist = await this.dataService
                    .CropPestDsses
                    .FindByConditionAsync(c =>
                        c.CropPestId == getFieldCropPest.CropPestId
                        & c.DssId== cropPestDssForCreationDto.DssId);

                if (cropPestDssExist == null)
                {
                    cropPestDssExist = new CropPestDss()
                    {
                        CropPestId = getFieldCropPest.CropPestId,
                        DssId = cropPestDssForCreationDto.DssId,
                        DssName = cropPestDssForCreationDto.DssId
                    };
                    this.dataService.CropPestDsses.Create(cropPestDssExist);
                }
                
                var newFieldCropPestDss = new FieldCropPestDss()
                {
                    FieldCropPestId = cropPestDssForCreationDto.FieldCropPestId,
                    CropPestDssId = cropPestDssExist.Id
                };
                this.dataService.FieldCropPestDsses.Create(newFieldCropPestDss);
                await this.dataService.CompleteAsync();

                var fieldCropPestToReturn = this.mapper
                    .Map<FieldCropPestDssDto>(newFieldCropPestDss)
                    .ShapeData() as IDictionary<string, object>;
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
                   .FieldCropPests
                   .Select(f => f.FieldCropPestDsses
                       .Where(fcpd => fcpd.Id == id).FirstOrDefault()).FirstOrDefault();

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
        
        #endregion
    }
}