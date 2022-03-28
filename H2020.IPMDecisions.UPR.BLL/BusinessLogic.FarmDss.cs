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
        public async Task<GenericResponse<IDictionary<string, object>>> AddListOfFarmDss(IEnumerable<FarmDssForCreationDto> listOfFarmDssDto, HttpContext httpContext, string mediaType)
        {
            try
            {
                if (!MediaTypeHeaderValue.TryParse(mediaType,
                      out MediaTypeHeaderValue parsedMediaType))
                    return GenericResponseBuilder.NoSuccess<IDictionary<string, object>>(null, this.jsonStringLocalizer["shared.wrong_media_type"].ToString());

                if (listOfFarmDssDto.Count() == 0) return GenericResponseBuilder.NoSuccess<IDictionary<string, object>>(null, this.jsonStringLocalizer["shared.missing_payload"].ToString());

                var farm = httpContext.Items["farm"] as Farm;
                var listOfNewFieldCropPestDss = new List<FieldCropPestDss>();
                var listToReturn = new List<FieldCropPestDssDto>();
                var listOfErrorsToReturn = new List<string>();
                var dataAsCropGroup = listOfFarmDssDto.GroupBy(f => new { f.CropEppoCode, f.FieldId }).ToList();
                foreach (var cropGroup in dataAsCropGroup)
                {
                    var fieldId = cropGroup.Key.FieldId;
                    var fieldAsEntity = farm
                        .Fields
                        .FirstOrDefault(
                            fi => fi.Id == fieldId ||
                            fi.FieldCrop.CropEppoCode == cropGroup.Key.CropEppoCode);
                    var fieldCrop = new FieldCrop();
                    if (fieldAsEntity == null)
                    {
                        var firstFarmForCreation = cropGroup.FirstOrDefault();
                        fieldAsEntity = AddFieldToFarm(firstFarmForCreation, farm);
                        fieldCrop = AddFieldCropToField(fieldAsEntity, cropGroup.Key.CropEppoCode);
                    }
                    else
                    {
                        if (fieldAsEntity.FieldCrop.CropEppoCode.ToUpper() != cropGroup.Key.CropEppoCode.ToUpper())
                        {
                            listOfErrorsToReturn.Add(this.jsonStringLocalizer["dss.field_eppo_code", fieldAsEntity.Id,
                                                    fieldAsEntity.FieldCrop.CropEppoCode].ToString());
                            continue;
                        }
                        fieldCrop = fieldAsEntity.FieldCrop;
                    }
                    var fieldCropPestExists = new FieldCropPest();
                    foreach (var farmForcreation in cropGroup)
                    {
                        fieldCropPestExists = await AddCropPestToFieldCrop(fieldCrop, farmForcreation.PestEppoCode);

                        var cropPestDss = this.mapper.Map<CropPestDss>(farmForcreation);
                        var newFieldCropPestDss = await CreateFieldCropPestDss(
                            fieldCropPestExists,
                            cropPestDss,
                            farmForcreation.DssParameters);
                        if (newFieldCropPestDss != null)
                            listOfNewFieldCropPestDss.Add(newFieldCropPestDss);
                        else
                            listOfErrorsToReturn.Add(this.jsonStringLocalizer["dss.duplicated",
                                farmForcreation.CropEppoCode,
                                farmForcreation.PestEppoCode,
                                farmForcreation.DssId, farmForcreation.DssModelId,
                                farmForcreation.DssModelVersion].ToString());
                    }
                }
                var dataToReturn = new Dictionary<string, object>();
                if (listOfErrorsToReturn.Count > 0)
                {
                    dataToReturn.Add("warnings", listOfErrorsToReturn);
                    var warningsAsString = this.jsonStringLocalizer["dss.warnings"].ToString();
                    foreach (var error in listOfErrorsToReturn)
                    {
                        warningsAsString = string.Format("{0} {1};", warningsAsString, error);
                    }
                    httpContext.Response.Headers.Add("warning", "Warning");
                    httpContext.Response.Headers.Add("warn-text", this.jsonStringLocalizer["dss.warning_header"].ToString());
                    return GenericResponseBuilder.Duplicated<IDictionary<string, object>>(warningsAsString, dataToReturn);
                }

                await this.dataService.CompleteAsync();
                foreach (var newDss in listOfNewFieldCropPestDss)
                {
                    var fieldCropPestDssToReturn = this.mapper.Map<FieldCropPestDssDto>(newDss);
                    if (newDss.CropPestDss.DssExecutionType.ToLower() == "onthefly")
                    {
                        var jobId = this.queueJobs.AddDssOnTheFlyQueue(newDss.Id);
                        fieldCropPestDssToReturn.DssTask.Id = jobId;
                        newDss.LastJobId = jobId;
                    }
                    listToReturn.Add(fieldCropPestDssToReturn);
                }
                await this.dataService.CompleteAsync();
                dataToReturn.Add("value", listToReturn);

                return GenericResponseBuilder.Success<IDictionary<string, object>>(dataToReturn);
            }
            catch (Exception ex)
            {
                logger.LogError(string.Format("Error in BLL - AddListOfFarmDss. {0}", ex.Message), ex);
                String innerMessage = (ex.InnerException != null) ? ex.InnerException.Message : "";
                return GenericResponseBuilder.NoSuccess<IDictionary<string, object>>(null, $"{ex.Message} InnerException: {innerMessage}");
            }
        }

        private async Task<GenericResponse<FieldCropPestDssDto>> AddNewFarmDss(FarmDssForCreationDto farmDssDto, HttpContext httpContext, string mediaType)
        {
            try
            {
                if (!MediaTypeHeaderValue.TryParse(mediaType,
                       out MediaTypeHeaderValue parsedMediaType))
                    return GenericResponseBuilder.NoSuccess<FieldCropPestDssDto>(null, this.jsonStringLocalizer["shared.wrong_media_type"].ToString());

                var farm = httpContext.Items["farm"] as Farm;
                var fieldAsEntity = farm.Fields.FirstOrDefault(fi => fi.Id == farmDssDto.FieldId);

                var fieldCropPestExists = new FieldCropPest();
                if (fieldAsEntity == null)
                {
                    fieldAsEntity = AddFieldToFarm(farmDssDto, farm);
                    fieldCropPestExists = await AddCropPestToField(farmDssDto, fieldAsEntity);
                }
                else
                {
                    fieldCropPestExists = CheckIfFieldHasCropPestCombination(fieldAsEntity, farmDssDto.CropEppoCode, farmDssDto.PestEppoCode);

                    if (fieldCropPestExists == null)
                    {
                        if (fieldAsEntity.FieldCrop.CropEppoCode.ToUpper() != farmDssDto.CropEppoCode.ToUpper())
                            return GenericResponseBuilder.Duplicated<FieldCropPestDssDto>(this.jsonStringLocalizer["dss.field_eppo_code", fieldAsEntity.FieldCrop.CropEppoCode].ToString());

                        fieldCropPestExists = await AddCropPestToField(farmDssDto, fieldAsEntity);
                    }
                    else
                    {
                        bool duplicatedCropPestDssRecord = HasCropPestDssCombination(fieldAsEntity, fieldCropPestExists.Id, farmDssDto.DssId, farmDssDto.DssModelId, farmDssDto.DssModelVersion);

                        if (duplicatedCropPestDssRecord)
                            return GenericResponseBuilder.Duplicated<FieldCropPestDssDto>(this.jsonStringLocalizer["dss.existing_combination",
                                farmDssDto.CropEppoCode,
                                farmDssDto.PestEppoCode,
                                farmDssDto.DssModelName].ToString());
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
                    var jobId = this.queueJobs.AddDssOnTheFlyQueue(newFieldCropPestDss.Id);
                    fieldCropPestDssToReturn.DssTask.Id = jobId;
                    newFieldCropPestDss.LastJobId = jobId;
                    await this.dataService.CompleteAsync();

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

        private static bool HasCropPestDssCombination(Field field, Guid fieldCropPestId, string dssId, string dssModelId, string dssModelVersion)
        {
            return field
                .FieldCrop
                .FieldCropPests
                .Any(f => f.FieldCropPestDsses
                    .Any(fcpd =>
                        fcpd.FieldCropPestId == fieldCropPestId
                        & fcpd.CropPestDss.DssId == dssId
                        & fcpd.CropPestDss.DssModelId == dssModelId));
        }

        private static FieldCropPest CheckIfFieldHasCropPestCombination(Field field, string cropEppoCode, string pestEppoCode)
        {
            return field
                .FieldCrop
                .FieldCropPests.Where(
                    fi => fi.CropPest.CropEppoCode.ToUpper() == cropEppoCode.ToUpper()
                    && fi.CropPest.PestEppoCode.ToUpper() == pestEppoCode.ToUpper())
                .FirstOrDefault();
        }

        private async Task<FieldCropPest> AddCropPestToField(FarmDssForCreationDto farmDssDto, Field field)
        {
            await AddCropPestToField(this.mapper.Map<CropPestForCreationDto>(farmDssDto), field);
            return field.FieldCrop.FieldCropPests.FirstOrDefault();
        }

        private Field AddFieldToFarm(FarmDssForCreationDto farmDssDto, Farm farm)
        {
            Field fieldAsEntity = this.mapper.Map<Field>(farmDssDto);
            fieldAsEntity.Farm = farm;
            this.dataService.Fields.Create(fieldAsEntity);
            return fieldAsEntity;
        }
    }
}