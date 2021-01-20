using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using H2020.IPMDecisions.UPR.BLL.Helpers;
using H2020.IPMDecisions.UPR.Core.Dtos;
using H2020.IPMDecisions.UPR.Core.Entities;
using H2020.IPMDecisions.UPR.Core.Helpers;
using H2020.IPMDecisions.UPR.Core.Models;
using H2020.IPMDecisions.UPR.Core.ResourceParameters;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Net.Http.Headers;

namespace H2020.IPMDecisions.UPR.BLL
{
    public partial class BusinessLogic : IBusinessLogic
    {
        public async Task<GenericResponse<IDictionary<string, object>>> AddNewFieldCropPest(CropPestForCreationDto cropPestForCreationDto, HttpContext httpContext, string mediaType)
        {
            try
            {
                var field = httpContext.Items["field"] as Field;

                var cropPestExist = await this.dataService.CropPests
                    .FindByConditionAsync
                    (c => c.CropEppoCode == cropPestForCreationDto.CropEppoCode
                    && c.PestEppoCode == cropPestForCreationDto.PestEppoCode);
                if (cropPestExist == null)
                {
                    cropPestExist = this.mapper.Map<CropPest>(cropPestForCreationDto);
                    this.dataService.CropPests.Create(cropPestExist);
                }

                var fieldCrop = new FieldCrop();
                if (field.FieldCrop == null)
                {
                    fieldCrop = new FieldCrop()
                    {
                        CropEppoCode = cropPestForCreationDto.CropEppoCode,
                        Field = field
                    };
                }
                else if (field.FieldCrop.CropEppoCode != cropPestForCreationDto.CropEppoCode)
                {
                    return GenericResponseBuilder.Duplicated<IDictionary<string, object>>();
                }
                else
                {
                    fieldCrop = field.FieldCrop;
                }

                var newFieldCropPest = new FieldCropPest()
                {
                    CropPest = cropPestExist,
                    FieldCrop = fieldCrop
                };
                this.dataService.FieldCropPests.Create(newFieldCropPest);
                await this.dataService.CompleteAsync();
                var cropPestToReturn = this.mapper
                    .Map<FieldCropWithChildrenDto>(fieldCrop)
                    .ShapeData() as IDictionary<string, object>;

                return GenericResponseBuilder.Success<IDictionary<string, object>>(cropPestToReturn);
            }
            catch (Exception ex)
            {
                logger.LogError(string.Format("Error in BLL - AddNewFieldCropPest. {0}", ex.Message), ex);
                String innerMessage = (ex.InnerException != null) ? ex.InnerException.Message : "";
                return GenericResponseBuilder.NoSuccess<IDictionary<string, object>>(null, $"{ex.Message} InnerException: {innerMessage}");
            }
        }

        public async Task<GenericResponse> DeleteFieldCropPest(Guid id, Guid fieldId, HttpContext httpContext)
        {
            try
            {
                var field = httpContext.Items["field"] as Field;
                if (field.FieldCrop == null)
                {
                    return GenericResponseBuilder.Success();
                }
                var fieldCropPestExist = await this.dataService
                        .FieldCropPests
                        .FindByConditionAsync(f =>
                            f.FieldCropId == field.FieldCrop.Id
                            & f.Id == id);

                if (fieldCropPestExist == null) return GenericResponseBuilder.Success();

                this.dataService.FieldCropPests.Delete(fieldCropPestExist);
                await this.dataService.CompleteAsync();

                return GenericResponseBuilder.Success();

            }
            catch (Exception ex)
            {
                logger.LogError(string.Format("Error in BLL - DeleteFieldCropPest. {0}", ex.Message), ex);
                String innerMessage = (ex.InnerException != null) ? ex.InnerException.Message : "";
                return GenericResponseBuilder.NoSuccess($"{ex.Message} InnerException: {innerMessage}");
            }
        }

        public async Task<GenericResponse<IDictionary<string, object>>> GetFieldCropPest(Guid id, Guid fieldId, string mediaType, HttpContext httpContext)
        {
            try
            {
                if (!MediaTypeHeaderValue.TryParse(mediaType,
                        out MediaTypeHeaderValue parsedMediaType))
                    return GenericResponseBuilder.NoSuccess<IDictionary<string, object>>(null, "Wrong media type.");

                var field = httpContext.Items["field"] as Field;

                if (field.FieldCrop == null)
                {
                    return GenericResponseBuilder.NotFound<IDictionary<string, object>>();
                }

                var fieldCropPestExist = await this.dataService
                    .FieldCropPests
                    .FindByConditionAsync(f =>
                        f.FieldCrop.FieldId == fieldId
                        & f.Id == id, true);

                if (fieldCropPestExist == null) return GenericResponseBuilder.NotFound<IDictionary<string, object>>();

                var cropPestToReturn = this.mapper
                    .Map<FieldCropPestDto>(fieldCropPestExist)
                    .ShapeData() as IDictionary<string, object>;

                return GenericResponseBuilder.Success<IDictionary<string, object>>(cropPestToReturn);

            }
            catch (Exception ex)
            {
                logger.LogError(string.Format("Error in BLL - GetFieldCropPest. {0}", ex.Message), ex);
                String innerMessage = (ex.InnerException != null) ? ex.InnerException.Message : "";
                return GenericResponseBuilder.NoSuccess<IDictionary<string, object>>(null, $"{ex.Message} InnerException: {innerMessage}");
            }
        }

        public async Task<GenericResponse<ShapedDataWithLinks>> GetFieldCropPests(Guid fieldId, FieldCropPestResourceParameter resourceParameter, string mediaType)
        {
            try
            {
                if (!MediaTypeHeaderValue.TryParse(mediaType,
                      out MediaTypeHeaderValue parsedMediaType))
                    return GenericResponseBuilder.NoSuccess<ShapedDataWithLinks>(null, "Wrong media type.");

                var fieldCropPestAsEntities = await this
                    .dataService
                    .FieldCropPests
                    .FindAllAsync(resourceParameter, fieldId, true);

                if (fieldCropPestAsEntities.Count == 0) return GenericResponseBuilder.NotFound<ShapedDataWithLinks>();

                var paginationMetaData = MiscellaneousHelper.CreatePaginationMetadata(fieldCropPestAsEntities);

                var links = UrlCreatorHelper.CreateLinksForFieldCropPests(
                    this.url,
                    fieldId,
                    resourceParameter,
                    fieldCropPestAsEntities.HasNext,
                    fieldCropPestAsEntities.HasPrevious);

                var shapedFieldCropPestToReturn = this.mapper
                    .Map<IEnumerable<FieldCropPestWithChildrenDto>>(fieldCropPestAsEntities)
                    .ShapeData();

                var dataToReturn = new ShapedDataWithLinks()
                {
                    Value = shapedFieldCropPestToReturn,
                    Links = links,
                    PaginationMetaData = paginationMetaData,
                };

                return GenericResponseBuilder.Success<ShapedDataWithLinks>(dataToReturn);
            }
            catch (Exception ex)
            {
                logger.LogError(string.Format("Error in BLL - GetFieldCropPests. {0}", ex.Message), ex);
                String innerMessage = (ex.InnerException != null) ? ex.InnerException.Message : "";
                return GenericResponseBuilder.NoSuccess<ShapedDataWithLinks>(null, $"{ex.Message} InnerException: {innerMessage}");
            }
        }

        #region Helpers

        #endregion
    }
}