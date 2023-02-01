using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using H2020.IPMDecisions.UPR.BLL.Helpers;
using H2020.IPMDecisions.UPR.Core.Dtos;
using H2020.IPMDecisions.UPR.Core.Entities;
using H2020.IPMDecisions.UPR.Core.Enums;
using H2020.IPMDecisions.UPR.Core.Models;
using Hangfire;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Schema;

namespace H2020.IPMDecisions.UPR.BLL
{
    public partial class BusinessLogic : IBusinessLogic
    {
        public async Task<GenericResponse<FieldDssResultDetailedDto>> GetFieldCropPestDssById(Guid id, Guid userId, int daysDataToReturn = 7)
        {
            try
            {
                var dss = await this.dataService.FieldCropPestDsses.FindByIdAsync(id);
                if (dss == null) return GenericResponseBuilder.NotFound<FieldDssResultDetailedDto>();

                var dssUserId = dss.FieldCropPest.FieldCrop.Field.Farm.UserFarms.FirstOrDefault().UserId;
                if (userId != dssUserId) return GenericResponseBuilder.NotFound<FieldDssResultDetailedDto>();

                FieldDssResultDetailedDto dataToReturn = await CreateDetailedResultToReturn(dss, daysDataToReturn);
                return GenericResponseBuilder.Success<FieldDssResultDetailedDto>(dataToReturn);
            }
            catch (Exception ex)
            {
                logger.LogError(string.Format("Error in BLL - GetFieldCropPestDssById. {0}", ex.Message), ex);
                String innerMessage = (ex.InnerException != null) ? ex.InnerException.Message : "";
                return GenericResponseBuilder.NoSuccess<FieldDssResultDetailedDto>(null, $"{ex.Message} InnerException: {innerMessage}");
            }
        }

        public async Task<GenericResponse> UpdateFieldCropPestDssById(Guid id, Guid userId, FieldCropPestDssForUpdateDto fieldCropPestDssForUpdateDto)
        {
            try
            {
                var dss = await this.dataService.FieldCropPestDsses.FindByIdAsync(id);
                if (dss == null) return GenericResponseBuilder.NotFound<FieldCropPestDssDto>();

                var dssUserId = dss.FieldCropPest.FieldCrop.Field.Farm.UserFarms.FirstOrDefault().UserId;
                if (userId != dssUserId) return GenericResponseBuilder.NotFound<FieldCropPestDssDto>();

                // Validate DSS parameters on server side
                var validationErrorMessages = await ValidateNewDssParameters(dss.CropPestDss, fieldCropPestDssForUpdateDto.DssParameters);
                if (validationErrorMessages.Count > 0)
                {
                    var errorMessageToReturn = string.Join(" ", validationErrorMessages);
                    return GenericResponseBuilder.NoSuccess(errorMessageToReturn);
                }

                this.mapper.Map(fieldCropPestDssForUpdateDto, dss);
                this.dataService.FieldCropPestDsses.Update(dss);
                await this.dataService.CompleteAsync();

                if (dss.CropPestDss.DssExecutionType.ToLower() == "onthefly")
                {
                    var jobId = this.queueJobs.AddDssOnTheFlyQueue(id);
                    dss.LastJobId = jobId;
                    await this.dataService.CompleteAsync();
                }
                return GenericResponseBuilder.Success();
            }
            catch (Exception ex)
            {
                logger.LogError(string.Format("Error in BLL - UpdateFieldCropPestDssById. {0}", ex.Message), ex);
                String innerMessage = (ex.InnerException != null) ? ex.InnerException.Message : "";
                return GenericResponseBuilder.NoSuccess($"{ex.Message} InnerException: {innerMessage}");
            }
        }

        public async Task<GenericResponse<IEnumerable<FieldDssResultDto>>> GetAllDssResults(Guid userId, bool? displayOutOfSeason)
        {
            try
            {
                var dssResults = await this.dataService.DssResult.GetAllDssResults(userId);
                if (dssResults == null) { return GenericResponseBuilder.Success<IEnumerable<FieldDssResultDto>>(new List<FieldDssResultDto>()); }
                var dssResultsToReturn = this.mapper.Map<IEnumerable<FieldDssResultDto>>(dssResults);

                if (dssResultsToReturn != null && dssResultsToReturn.Count() != 0)
                {
                    bool newDisplayOutOfSeason = displayOutOfSeason.HasValue ? displayOutOfSeason.Value : bool.Parse(this.config["AppConfiguration:DisplayOutOfSeasonDss"]);
                    await AddExtraInformationToDss(dssResultsToReturn, newDisplayOutOfSeason);
                }
                return GenericResponseBuilder.Success<IEnumerable<FieldDssResultDto>>(dssResultsToReturn);
            }
            catch (Exception ex)
            {
                logger.LogError(string.Format("Error in BLL - GetAllUserFieldCropPestDss. {0}", ex.Message), ex);
                String innerMessage = (ex.InnerException != null) ? ex.InnerException.Message : "";
                return GenericResponseBuilder.NoSuccess<IEnumerable<FieldDssResultDto>>(null, $"{ex.Message} InnerException: {innerMessage}");
            }
        }

        public async Task<GenericResponse<IEnumerable<LinkDssDto>>> GetAllLinkDss(Guid userId)
        {
            try
            {
                List<LinkDssDto> dataToReturn = new List<LinkDssDto>();
                var farmsFromUser = await this.dataService.Farms.FindAllByConditionAsync(f => f.UserFarms.Any(uf => uf.UserId == userId & (uf.Authorised)));
                if (farmsFromUser.Count() == 0) return GenericResponseBuilder.Success<IEnumerable<LinkDssDto>>(dataToReturn);

                var farmGeoJson = CreateFarmLocationGeoJson(farmsFromUser);
                var listDssOnLocation = await this.internalCommunicationProvider.GetListOfDssByLocationFromDssMicroservice(farmGeoJson, "LINK");

                if (listDssOnLocation == null) return GenericResponseBuilder.Success<IEnumerable<LinkDssDto>>(dataToReturn);

                // Only DSS that have models with links and crops from farms
                var farmCrops = farmsFromUser
                    .SelectMany(f => f.Fields.Select(fi => fi.FieldCrop.CropEppoCode))
                    .Distinct()
                    .ToList();

                List<DssInformationJoined> linkDssOnLocation = new List<DssInformationJoined>();
                foreach (var crop in farmCrops)
                {
                    var linkDssOnCrop = listDssOnLocation
                    .SelectMany(d => d.DssModelInformation, (dss, model) => new DssInformationJoined { DssInformation = dss, DssModelInformation = model })
                    .Where(linkDssFiltered => linkDssFiltered.DssModelInformation.Execution.Type.ToLower() == "link"
                        & linkDssFiltered.DssModelInformation.Crops.Any(c => c.Equals(crop)))
                    .ToList();

                    if (linkDssOnCrop.Count() == 0) continue;
                    linkDssOnCrop.ForEach(l =>
                    {
                        var oldIndex = l.DssModelInformation.Crops.FindIndex(c => c.Equals(crop));
                        l.DssModelInformation.Crops.RemoveAt(oldIndex);
                        l.DssModelInformation.Crops.Insert(0, crop);
                        var mappedItem = this.mapper.Map<LinkDssDto>(l);
                        dataToReturn.Add(mappedItem);
                    });
                    linkDssOnLocation.AddRange(linkDssOnCrop);
                }
                if (linkDssOnLocation.Count() == 0) return GenericResponseBuilder.Success<IEnumerable<LinkDssDto>>(dataToReturn);

                var eppoCodesData = await this.dataService.EppoCodes.GetEppoCodesAsync();
                foreach (var dss in dataToReturn)
                {
                    var eppoCodeLanguages = EppoCodesHelper.GetCropPestEppoCodesNames(eppoCodesData, dss.CropEppoCode, dss.PestEppoCode);
                    dss.CropLanguages = eppoCodeLanguages.CropLanguages;
                    dss.PestLanguages = eppoCodeLanguages.PestLanguages;
                }
                return GenericResponseBuilder.Success<IEnumerable<LinkDssDto>>(dataToReturn);
            }
            catch (Exception ex)
            {
                logger.LogError(string.Format("Error in BLL - GetAllLinkDss. {0}", ex.Message), ex);
                String innerMessage = (ex.InnerException != null) ? ex.InnerException.Message : "";
                return GenericResponseBuilder.NoSuccess<IEnumerable<LinkDssDto>>(null, $"{ex.Message} InnerException: {innerMessage}");
            }
        }

        public async Task<GenericResponse<JObject>> GetFieldCropPestDssParametersById(Guid id, Guid userId, bool? displayInternalParameters)
        {
            try
            {
                var dss = await this.dataService.FieldCropPestDsses.FindByIdAsync(id);
                if (dss == null) return GenericResponseBuilder.NotFound<JObject>();

                var dssUserId = dss.FieldCropPest.FieldCrop.Field.Farm.UserFarms.FirstOrDefault().UserId;
                if (userId != dssUserId) return GenericResponseBuilder.NotFound<JObject>();

                bool newDisplayInternalParameters = displayInternalParameters.HasValue ? displayInternalParameters.Value : bool.Parse(this.config["AppConfiguration:DisplayInternalParameters"]);
                var dataToReturn = await GenerateUserDssParameters(dss, newDisplayInternalParameters);
                return GenericResponseBuilder.Success<JObject>(dataToReturn);
            }
            catch (Exception ex)
            {
                logger.LogError(string.Format("Error in BLL - GetFieldCropPestDssParametersById. {0}", ex.Message), ex);
                String innerMessage = (ex.InnerException != null) ? ex.InnerException.Message : "";
                return GenericResponseBuilder.NoSuccess<JObject>(null, $"{ex.Message} InnerException: {innerMessage}");
            }
        }

        public async Task<GenericResponse<JObject>> GetFieldCropPestDssDefaultParametersById(Guid id, Guid userId)
        {
            try
            {
                var dss = await this.dataService.FieldCropPestDsses.FindByIdAsync(id);
                if (dss == null) return GenericResponseBuilder.NotFound<JObject>();

                var dssUserId = dss.FieldCropPest.FieldCrop.Field.Farm.UserFarms.FirstOrDefault().UserId;
                if (userId != dssUserId) return GenericResponseBuilder.NotFound<JObject>();

                var dssParameters = await GetDefaultDssParametersFromMicroservice(dss.CropPestDss);
                // ToDo: Return only JSON or get new default parameters and overwrite?
                // dss.DssParameters = dssParameters;
                // await this.dataService.CompleteAsync();
                JObject dataToReturn = JObject.Parse(dssParameters.ToString());
                return GenericResponseBuilder.Success<JObject>(dataToReturn);
            }
            catch (Exception ex)
            {
                logger.LogError(string.Format("Error in BLL - GetFieldCropPestDssDefaultParametersById. {0}", ex.Message), ex);
                String innerMessage = (ex.InnerException != null) ? ex.InnerException.Message : "";
                return GenericResponseBuilder.NoSuccess<JObject>(null, $"{ex.Message} InnerException: {innerMessage}");
            }
        }

        private async Task AddExtraInformationToDss(IEnumerable<FieldDssResultDto> dssResultsToReturn, bool outOfSeason = false)
        {
            try
            {
                var listOfDss = await this.internalCommunicationProvider.GetAllListOfDssFromDssMicroservice();
                var eppoCodesData = await this.dataService.EppoCodes.GetEppoCodesAsync();
                var monitoringApi = JobStorage.Current.GetMonitoringApi();

                foreach (var dss in dssResultsToReturn)
                {
                    if (listOfDss != null && listOfDss.Count() != 0)
                    {
                        var dssOnListMatchDatabaseRecord = listOfDss
                            .Where(d => d.Id == dss.DssId)
                            .FirstOrDefault();
                        if (dssOnListMatchDatabaseRecord == null) continue;

                        var dssModelMatchDatabaseRecord = dssOnListMatchDatabaseRecord
                          .DssModelInformation
                          .Where(dm => dm.Id == dss.DssModelId)
                           .FirstOrDefault();

                        if (dssModelMatchDatabaseRecord == null)
                        {
                            dss.DssDescription = this.jsonStringLocalizer["dss.model_missing_metadata",
                                dss.DssId,
                                dss.DssVersion,
                                dss.DssModelId,
                                dss.DssModelVersion].ToString();
                        }
                        else
                        {
                            var dssApiUrl = config["MicroserviceInternalCommunication:DssApiUrl"];
                            this.mapper.Map(dssOnListMatchDatabaseRecord, dss, opt =>
                            {
                                opt.Items["host"] = string.Format("{0}", dssApiUrl);
                            });

                            this.mapper.Map(dssModelMatchDatabaseRecord, dss);
                            if (dssModelMatchDatabaseRecord.Output != null)
                            {
                                AddWarningMessages(dss, dssModelMatchDatabaseRecord);
                            }
                            if (!outOfSeason) CheckIfDssOutOfSeason(dss);
                        }
                    }

                    if (eppoCodesData != null && eppoCodesData.Count > 0)
                    {
                        var eppoCodeLanguages = EppoCodesHelper.GetCropPestEppoCodesNames(eppoCodesData, dss.CropEppoCode, dss.PestEppoCode);
                        dss.CropLanguages = eppoCodeLanguages.CropLanguages;
                        dss.PestLanguages = eppoCodeLanguages.PestLanguages;
                    }
                    if (monitoringApi == null || string.IsNullOrEmpty(dss.DssTaskStatusDto.Id)) continue;
                    var jobDetail = monitoringApi.JobDetails(dss.DssTaskStatusDto.Id);
                    if (jobDetail != null)
                    {
                        dss.DssTaskStatusDto = CreateDssStatusFromJobDetail(dss.Id, dss.DssTaskStatusDto.Id, jobDetail);
                    }
                }
            }
            catch (Exception ex)
            {
                logger.LogError(string.Format("Error in BLL - AddExtraInformationToDss. {0}", ex.Message), ex);
                throw ex;
            }
        }

        private void CheckIfDssOutOfSeason(FieldDssResultDto dss)
        {
            var fullResult = JsonConvert.DeserializeObject<DssModelOutputInformation>(dss.DssFullResult);
            if (fullResult.TimeEnd != null && DateTime.Parse(fullResult.TimeEnd) < DateTime.Today)
            {
                var dateTimeAsShortDate = DateTime.Parse(fullResult.TimeEnd).ToString("dd/MM/yyyy");
                dss.IsValid = false;
                dss.ResultMessageType = (int)DssOutputMessageTypeEnum.Info;
                dss.ResultMessage = this.jsonStringLocalizer["weather.end_of_season", dateTimeAsShortDate].ToString();
                dss.WarningStatus = 0;
            }
        }

        public async Task<GenericResponse> DeleteDss(Guid id, Guid userId)
        {
            try
            {
                var dss = await this.dataService.FieldCropPestDsses.FindByIdAsync(id);
                if (dss == null) return GenericResponseBuilder.Success();

                if (!dss.FieldCropPest.FieldCrop.Field.Farm.UserFarms.Any(u => u.UserId == userId))
                {
                    return GenericResponseBuilder.Success();
                }

                if (dss.FieldCropPest.FieldCrop.Field.Farm.UserFarms.Count > 1)
                {
                    // ToDo: Ask for requirements, only farm owners can delete data??
                }

                // If last FieldCropPestDss and last FieldCropPest delete field
                if (dss.FieldCropPest.FieldCropPestDsses.Count() == 1
                    && dss.FieldCropPest.FieldCrop.FieldCropPests.Count() == 1)
                {
                    var field = dss.FieldCropPest.FieldCrop.Field;
                    this.dataService.Fields.Delete(field);
                }
                // If last FieldCropPestDss on FieldCropPest, but extra existing FieldCropPest, delete the FieldCropPest
                else if (dss.FieldCropPest.FieldCropPestDsses.Count() == 1
                    && dss.FieldCropPest.FieldCrop.FieldCropPests.Count() > 1)
                {
                    var fieldCropPest = dss.FieldCropPest;
                    this.dataService.FieldCropPests.Delete(fieldCropPest);
                }
                // If more than one FieldCropPestDss, delete just FieldCropPestDss
                else
                {
                    this.dataService.FieldCropPestDsses.Delete(dss);
                }

                await this.dataService.CompleteAsync();
                return GenericResponseBuilder.Success();
            }
            catch (Exception ex)
            {
                logger.LogError(string.Format("Error in BLL - DeleteDss. {0}", ex.Message), ex);
                String innerMessage = (ex.InnerException != null) ? ex.InnerException.Message : "";
                return GenericResponseBuilder.NoSuccess($"{ex.Message} InnerException: {innerMessage}");
            }
        }

        private async Task<FieldDssResultDetailedDto> CreateDetailedResultToReturn(FieldCropPestDss dss, int daysDataToReturn = 7)
        {
            var dataToReturn = this.mapper.Map<FieldDssResultDetailedDto>(dss);
            var dssInformation = await internalCommunicationProvider
                            .GetDssInformationFromDssMicroservice(dss.CropPestDss.DssId);

            if (dssInformation == null) return dataToReturn;
            dataToReturn.DssSource = string.Format("{0}, {1}",
                            dssInformation.DssOrganization.Name,
                            dssInformation.DssOrganization.Country);
            dataToReturn.DssVersion = dssInformation.Version;
            if (!string.IsNullOrEmpty(dssInformation.LogoUrl))
            {
                var dssApiUrl = config["MicroserviceInternalCommunication:DssApiUrl"];
                dataToReturn.DssLogoUrl = string.Format("{0}{1}",
                    dssApiUrl,
                    dssInformation.LogoUrl);
            }
            var dssModelInformation = dssInformation
                .DssModelInformation
                .FirstOrDefault(m => m.Id == dss.CropPestDss.DssModelId);
            if (dssModelInformation == null) return dataToReturn;
            AddDssBasicData(dataToReturn, dssModelInformation);
            await AddDssCropPestNames(dataToReturn);

            if (!dataToReturn.IsValid || dataToReturn.DssExecutionType.ToLower() == "link") return dataToReturn;
            DssModelOutputInformation dssFullOutputAsObject = AddDssFullResultData(dataToReturn);

            var locationResultData = dssFullOutputAsObject.LocationResult.FirstOrDefault();
            int maxDaysOutput = int.Parse(this.config["AppConfiguration:MaxDaysAllowedForDssOutputData"]);
            IEnumerable<List<double?>> dataLastDays = SelectDssLastResultsData(dataToReturn, locationResultData, daysDataToReturn, maxDaysOutput);
            List<string> labels = CreateResultParametersLabels(dataToReturn.OutputTimeEnd, dataLastDays.Count());
            if (labels == null || labels.Count() == 0)
            {
                if (daysDataToReturn > maxDaysOutput) daysDataToReturn = maxDaysOutput;
                labels = CreateResultParametersLabels(dataToReturn.OutputTimeEnd, daysDataToReturn);
            }
            dataToReturn.WarningStatusLabels = labels;

            for (int i = 0; i < dssFullOutputAsObject.ResultParameters.Count; i++)
            {
                var parameterCode = dssFullOutputAsObject.ResultParameters[i];
                var resultParameter = new ResultParameters();
                resultParameter.Labels = labels;

                resultParameter.Code = parameterCode;
                var parameterInformationFromDss = dssModelInformation
                        .Output
                        .ResultParameters
                        .Where(n => n.Id == parameterCode)
                        .FirstOrDefault();
                if (parameterInformationFromDss != null)
                {
                    resultParameter.Title = parameterInformationFromDss.Title;
                    resultParameter.Description = parameterInformationFromDss.Description;

                    if (parameterInformationFromDss.ChartInfo != null)
                    {
                        resultParameter.ChartInformation = this.mapper.Map<DssParameterChartInformation>(parameterInformationFromDss.ChartInfo);
                    };
                }

                foreach (var dataForParameters in dataLastDays)
                {
                    var data = dataForParameters[i];
                    resultParameter.Data.Add(data);
                }
                dataToReturn.ResultParameters.Add(resultParameter);
            }
            foreach (var group in dssModelInformation.Output.ChartGroups)
            {
                var chartGroupWithDataParameters = this.mapper.Map<ChartGroup>(group);

                chartGroupWithDataParameters
                    .ResultParameterIds
                    .ForEach(id =>
                    {
                        var resultParameterOnChartGroup = dataToReturn
                            .ResultParameters
                            .Where(rp => rp.Code == id)
                            .FirstOrDefault();
                        if (resultParameterOnChartGroup != null)
                            chartGroupWithDataParameters.ResultParameters.Add(resultParameterOnChartGroup);
                    });
                dataToReturn.ChartGroups.Add(chartGroupWithDataParameters);
            }
            return dataToReturn;
        }

        private async Task AddDssCropPestNames(FieldDssResultDetailedDto dataToReturn)
        {
            var eppoCodesData = await this.dataService.EppoCodes.GetEppoCodesAsync();

            var eppoCodeLanguages = EppoCodesHelper.GetCropPestEppoCodesNames(eppoCodesData, dataToReturn.CropEppoCode, dataToReturn.PestEppoCode);
            dataToReturn.CropLanguages = eppoCodeLanguages.CropLanguages;
            dataToReturn.PestLanguages = eppoCodeLanguages.PestLanguages;
        }

        private static IEnumerable<List<double?>> SelectDssLastResultsData(
            FieldDssResultDetailedDto dataToReturn,
            LocationResultDssOutput locationResultData,
            int daysOutput,
            int maxDaysOutput = 30)
        {
            IEnumerable<List<double?>> dataLastDays = new List<List<double?>>();
            if (daysOutput > maxDaysOutput) daysOutput = maxDaysOutput;

            if (locationResultData != null)
            {
                dataLastDays = locationResultData.Data.TakeLast(daysOutput);
                dataToReturn.ResultParametersLength = dataLastDays.Count();
                dataToReturn.WarningStatusPerDay = locationResultData.WarningStatus.TakeLast(daysOutput).ToList();
            };
            return dataLastDays;
        }

        private static DssModelOutputInformation AddDssFullResultData(FieldDssResultDetailedDto dataToReturn)
        {
            var dssFullOutputAsObject = JsonConvert.DeserializeObject<DssModelOutputInformation>(dataToReturn.DssFullResult);
            dataToReturn.OutputTimeStart = dssFullOutputAsObject.TimeStart;
            dataToReturn.OutputTimeEnd = dssFullOutputAsObject.TimeEnd;
            dataToReturn.Interval = dssFullOutputAsObject.Interval;
            dataToReturn.ResultParametersWidth = dssFullOutputAsObject.ResultParameters.Count;
            return dssFullOutputAsObject;
        }

        private void AddDssBasicData(FieldDssResultDetailedDto dataToReturn, DssModelInformation dssInformation)
        {
            this.mapper.Map(dssInformation, dataToReturn);

            // DSS type link do not have this section
            if (dssInformation.Output != null)
            {
                AddWarningMessages(dataToReturn, dssInformation);
            }
        }

        private List<string> CreateResultParametersLabels(string outputTimeEnd, int days)
        {
            if (outputTimeEnd is null) return null;

            var isADate = DateTime.TryParse(outputTimeEnd, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime dateTime);
            if (!isADate) return null;

            var labelsList = new List<string>();
            for (int i = days - 1; i >= 0; i--)
            {
                labelsList.Add(dateTime.AddDays(-i).ToString("dd/MM/yyyy", DateTimeFormatInfo.InvariantInfo));
            }
            return labelsList;
        }

        private static void AddWarningMessages(FieldDssResultBaseDto dss, DssModelInformation dssModelInformation)
        {
            dss.WarningStatusRepresentation = dssModelInformation.Output.ListWarningStatusInterpretation[dss.WarningStatus].Explanation;
            dss.WarningMessage = dssModelInformation.Output.ListWarningStatusInterpretation[dss.WarningStatus].RecommendedAction;
        }

        private static GeoJsonFeatureCollection CreateFarmLocationGeoJson(IEnumerable<Farm> farmsFromUser)
        {
            var geoJson = new GeoJsonFeatureCollection();
            foreach (var farm in farmsFromUser)
            {
                var coordinates = new List<double>(){
                    farm.Location.Coordinate.X,
                    farm.Location.Coordinate.Y
                };
                var geometry = new GeoJsonGeometry()
                {
                    Coordinates = coordinates
                };
                var feature = new GeoJsonFeature()
                {
                    Geometry = geometry
                };
                geoJson.Features.Add(feature);
            }
            return geoJson;
        }

        private async Task<IList<string>> ValidateNewDssParameters(CropPestDss cropPestDss, string newDssParameters)
        {
            try
            {
                var dssInputUISchema = await internalCommunicationProvider
                                       .GetDssModelInputSchemaMicroservice(cropPestDss.DssId, cropPestDss.DssModelId);
                DssDataHelper.RemoveNotRequiredInputSchemaProperties(dssInputUISchema);
                JObject inputAsJsonObject = JObject.Parse(newDssParameters.ToString());
                IList<string> validationErrorMessages;
                var isJsonObjectValid = inputAsJsonObject.IsValid(dssInputUISchema, out validationErrorMessages);
                return validationErrorMessages;
            }
            catch (Exception ex)
            {
                logger.LogError(string.Format("Error in BLL - ValidateNewDssParameters. {0}", ex.Message), ex);
                String innerMessage = (ex.InnerException != null) ? ex.InnerException.Message : "";
                throw ex;
            }
        }

        private async Task<JObject> GenerateUserDssParameters(FieldCropPestDss dss, bool displayInternalParameters = false)
        {
            var dssInputUISchema = await internalCommunicationProvider
                                                   .GetDssModelInputSchemaMicroservice(dss.CropPestDss.DssId, dss.CropPestDss.DssModelId);
            JObject inputAsJsonObject = null;
            if (dssInputUISchema != null)
            {
                inputAsJsonObject = JObject.Parse(dssInputUISchema.ToString());
                JObject userParametersAsJsonObject = JObject.Parse(dss.DssParameters.ToString());
                DssDataHelper.AddDefaultDssParametersToInputSchema(inputAsJsonObject, userParametersAsJsonObject);

                if (!displayInternalParameters)
                {
                    var dssModelInformation = await internalCommunicationProvider
                                            .GetDssModelInformationFromDssMicroservice(dss.CropPestDss.DssId, dss.CropPestDss.DssModelId);

                    var dssInternalParameters = dssModelInformation.Execution.InputSchemaCategories.Internal;

                    if (dssInternalParameters.Count != 0)
                    {
                        DssDataHelper.HideInternalDssParametersFromInputSchema(inputAsJsonObject, dssInternalParameters);
                    }
                }
            }
            return inputAsJsonObject;
        }
    }
}