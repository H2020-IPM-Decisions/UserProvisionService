using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using H2020.IPMDecisions.UPR.Core.Dtos;
using H2020.IPMDecisions.UPR.Core.Entities;
using H2020.IPMDecisions.UPR.Core.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Net.Http.Headers;

namespace H2020.IPMDecisions.UPR.BLL
{
    public partial class BusinessLogic : IBusinessLogic
    {
        public async Task<GenericResponse> AddListOfFarmDss(IEnumerable<FarmDssForCreationDto> listOfFarmDssDto, HttpContext httpContext, string mediaType)
        {
            try
            {
                if (!MediaTypeHeaderValue.TryParse(mediaType,
                      out MediaTypeHeaderValue parsedMediaType))
                    return GenericResponseBuilder.NoSuccess("Wrong media type.");

                var test = await AddNewFarmDss(listOfFarmDssDto.FirstOrDefault(), httpContext, mediaType);

                return GenericResponseBuilder.Success();
            }
            catch (Exception ex)
            {
                logger.LogError(string.Format("Error in BLL - AddListOfFarmDss. {0}", ex.Message), ex);
                String innerMessage = (ex.InnerException != null) ? ex.InnerException.Message : "";
                return GenericResponseBuilder.NoSuccess<FieldCropPestDssDto>(null, $"{ex.Message} InnerException: {innerMessage}");
            }
        }

        public async Task<GenericResponse<FieldCropPestDssDto>> AddNewFarmDss(FarmDssForCreationDto farmDssDto, HttpContext httpContext, string mediaType)
        {
            try
            {
                if (!MediaTypeHeaderValue.TryParse(mediaType,
                       out MediaTypeHeaderValue parsedMediaType))
                    return GenericResponseBuilder.NoSuccess<FieldCropPestDssDto>(null, "Wrong media type.");

                var farm = httpContext.Items["farm"] as Farm;
                var fieldAsEntity = farm.Fields.FirstOrDefault(fi => fi.Id == farmDssDto.FieldId);

                var fieldCropPestExists = new FieldCropPest();
                if (fieldAsEntity == null)
                {
                    fieldAsEntity = this.mapper.Map<Field>(farmDssDto);
                    fieldAsEntity.Farm = farm;
                    this.dataService.Fields.Create(fieldAsEntity);

                    await AddCropPestToField(this.mapper.Map<CropPestForCreationDto>(farmDssDto), fieldAsEntity);
                    fieldCropPestExists = fieldAsEntity.FieldCrop.FieldCropPests.FirstOrDefault();
                }
                else
                {
                    fieldCropPestExists = fieldAsEntity
                        .FieldCrop
                        .FieldCropPests.Where(
                            fi => fi.CropPest.CropEppoCode == farmDssDto.CropEppoCode.ToUpper()
                            && fi.CropPest.PestEppoCode == farmDssDto.PestEppoCode.ToUpper())
                            .FirstOrDefault();

                    if (fieldCropPestExists == null)
                    {
                        if (fieldAsEntity.FieldCrop.CropEppoCode.ToUpper() != farmDssDto.CropEppoCode.ToUpper())
                        {
                            return GenericResponseBuilder.Duplicated<FieldCropPestDssDto>(string.Format("Field only accepts '{0}' crop EPPO code", fieldAsEntity.FieldCrop.CropEppoCode));
                        }
                        await AddCropPestToField(this.mapper.Map<CropPestForCreationDto>(farmDssDto), fieldAsEntity);
                        fieldCropPestExists = fieldAsEntity.FieldCrop.FieldCropPests.FirstOrDefault();
                    }
                    else
                    {
                        var duplicatedCropPestDssRecord = fieldAsEntity
                                            .FieldCrop
                                            .FieldCropPests
                                            .Any(f => f.FieldCropPestDsses
                                                .Any(fcpd =>
                                                    fcpd.FieldCropPestId == fieldCropPestExists.Id
                                                    & fcpd.CropPestDss.DssId == farmDssDto.DssId
                                                    & fcpd.CropPestDss.DssModelId == farmDssDto.DssModelId));
                        if (duplicatedCropPestDssRecord)
                            return GenericResponseBuilder.Duplicated<FieldCropPestDssDto>(string
                                .Format("Field already has crop ({0}), pest ({1}), and DSS ({2}) combination.",
                                farmDssDto.CropEppoCode,
                                farmDssDto.PestEppoCode,
                                farmDssDto.DssModelName));
                    }
                }
                var cropPestDss = this.mapper.Map<CropPestDss>(farmDssDto);
                var newFieldCropPestDss = await CreateFieldCropPestDss(
                    fieldCropPestExists,
                    cropPestDss,
                    farmDssDto.DssParameters);
                await this.dataService.CompleteAsync();
                var fieldCropPestDssToReturn = this.mapper.Map<FieldCropPestDssDto>(newFieldCropPestDss);

                if (farmDssDto.DssExecutionType.ToLower() == "onthefly")
                {
                    var jobId = this.queueJobs.AddDssOnOnTheFlyQueue(newFieldCropPestDss.Id);
                    return GenericResponseBuilder.Accepted<FieldCropPestDssDto>(fieldCropPestDssToReturn);
                }
                return GenericResponseBuilder.Success<FieldCropPestDssDto>(fieldCropPestDssToReturn);
            }
            catch (Exception ex)
            {
                logger.LogError(string.Format("Error in BLL - AddNewFarmDss. {0}", ex.Message), ex);
                String innerMessage = (ex.InnerException != null) ? ex.InnerException.Message : "";
                return GenericResponseBuilder.NoSuccess<FieldCropPestDssDto>(null, $"{ex.Message} InnerException: {innerMessage}");
            }
        }
    }
}