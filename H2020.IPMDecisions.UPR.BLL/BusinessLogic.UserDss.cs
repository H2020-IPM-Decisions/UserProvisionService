using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using CsvHelper;
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
        public async Task<GenericResponse<FieldDssResultDetailedDto>> GetFieldCropPestDssById(Guid id, Guid userId, int daysDataToReturn)
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
                // Check if has field observations
                var hasFieldObservations = fieldCropPestDssForUpdateDto.DssParameters.Contains("fieldObservations", StringComparison.OrdinalIgnoreCase);
                if (hasFieldObservations)
                {
                    var userParametersAsJsonObject = JObject.Parse(fieldCropPestDssForUpdateDto.DssParameters);
                    var fieldObservation = CreateFieldObservationNoTime(dss);
                    DssDataHelper.UpdateFieldObservationWithNoTimeProperty(userParametersAsJsonObject, fieldObservation);
                    fieldCropPestDssForUpdateDto.DssParameters = userParametersAsJsonObject.ToString();
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

        private static DssModelFieldObservationNoTime CreateFieldObservationNoTime(FieldCropPestDss dss)
        {
            var fieldObservation = new DssModelFieldObservationNoTime();
            var coordinates = new List<double>(){
                        dss.FieldCropPest.FieldCrop.Field.Farm.Location.Coordinate.X,
                        dss.FieldCropPest.FieldCrop.Field.Farm.Location.Coordinate.Y
                    };
            var geometry = new GeoJsonGeometry()
            {
                Coordinates = coordinates
            };
            fieldObservation.Location = geometry;
            fieldObservation.PestEppoCode = dss.FieldCropPest.CropPest.PestEppoCode;
            fieldObservation.CropEppoCode = dss.FieldCropPest.CropPest.CropEppoCode;
            return fieldObservation;
        }

        public async Task<GenericResponse<IEnumerable<FieldDssResultDto>>> GetAllDssResults(Guid userId, DssResultListFilterDto filterDto)
        {
            try
            {
                var dssResults = await this.dataService.DssResult.GetAllDssResults(userId);
                if (dssResults == null) return GenericResponseBuilder.Success<IEnumerable<FieldDssResultDto>>(new List<FieldDssResultDto>());
                if (!string.IsNullOrEmpty(filterDto.ExecutionType))
                {
                    dssResults = dssResults.Where(d => d.DssExecutionType.ToLower() == filterDto.ExecutionType.ToLower()).ToList();
                }
                foreach (var result in dssResults)
                {
                    CheckLastInputUpdate(result);
                }

                var dssResultsToReturn = this.mapper.Map<IEnumerable<FieldDssResultDto>>(dssResults);

                if (dssResultsToReturn != null && dssResultsToReturn.Count() != 0)
                {
                    bool newDisplayOutOfSeason = filterDto.DisplayOutOfSeason.HasValue ? filterDto.DisplayOutOfSeason.Value : bool.Parse(this.config["AppConfiguration:DisplayOutOfSeasonDss"]);
                    var listUserFarmsAsOwner = await this.dataService.UserFarms
                        .FindAllAsync(uf => uf.UserId == userId & uf.UserFarmType.Id == UserFarmTypeEnum.Owner);
                    var listFarmsIdsAsOwner = listUserFarmsAsOwner.Select(uf => uf.FarmId).ToList();
                    await AddExtraInformationToDss(dssResultsToReturn, newDisplayOutOfSeason, listFarmsIdsAsOwner);
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

        private void CheckLastInputUpdate(DssResultDatabaseView result)
        {
            if (result.DssId.Equals("dk.seges") && result.DssModelId.StartsWith("CPO_"))
            {
                var daysSinceLastUpdate = (DateTime.Today - result.DssParametersLastUpdate).Days;
                if (result.IsValid == true && daysSinceLastUpdate >= 10)
                {
                    result.IsValid = false;
                    result.WarningStatus = 0;
                    result.ResultMessageType = 2;
                    result.ResultMessage = this.jsonStringLocalizer["dss.update_overdue"].ToString();
                }
                else if (result.IsValid == true && daysSinceLastUpdate >= 5)
                {
                    result.ResultMessageType = 1;
                    result.ResultMessage = this.jsonStringLocalizer["dss.update_needed", daysSinceLastUpdate].ToString();
                }
            }
        }

        public async Task<GenericResponse<IEnumerable<LinkDssDto>>> GetAllLinkDss(Guid userId)
        {
            try
            {
                List<LinkDssDto> dataToReturn = new List<LinkDssDto>();
                var farmsFromUser = await this.dataService.Farms.FindAllByConditionAsync(f => f.UserFarms.Any(uf => uf.UserId == userId & (uf.Authorised)));
                if (farmsFromUser.Count() == 0) return GenericResponseBuilder.Success<IEnumerable<LinkDssDto>>(dataToReturn);

                var farmGeoJson = CreateFarmsLocationGeoJson(farmsFromUser);
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

        public async Task<GenericResponse<byte[]>> GetFieldCropPestDssDataAsCSVById(Guid id, Guid userId)
        {
            try
            {
                var dss = await this.dataService.FieldCropPestDsses.FindByIdAsync(id);
                if (dss == null) return GenericResponseBuilder.NotFound<byte[]>();

                var dssUserId = dss.FieldCropPest.FieldCrop.Field.Farm.UserFarms.FirstOrDefault().UserId;
                if (userId != dssUserId) return GenericResponseBuilder.NotFound<byte[]>();


                var dataToReturn = this.mapper.Map<FieldDssResultDetailedDto>(dss);
                var dssInformation = await internalCommunicationProvider
                                .GetDssInformationFromDssMicroservice(dss.CropPestDss.DssId);
                if (dssInformation == null) return null;
                var dssModelInformation = dssInformation
                    .DssModelInformation
                    .FirstOrDefault(m => m.Id == dss.CropPestDss.DssModelId);

                if (!dataToReturn.IsValid || dataToReturn.DssExecutionType.ToLower() == "link") return null;
                DssModelOutputInformation dssFullOutputAsObject = AddDssFullResultData(dataToReturn);
                bool isHourlyInterval = dssFullOutputAsObject.Interval == "3600";

                using (var memoryStream = new MemoryStream())
                using (var writer = new StreamWriter(memoryStream))
                using (var csv = new CsvWriter(writer, CultureInfo.CurrentUICulture))
                {
                    csv.WriteField("Date");
                    csv.WriteField("WarningStatus");
                    foreach (string parameterCode in dssFullOutputAsObject.ResultParameters)
                    {
                        if (dssModelInformation == null || dssModelInformation.Output == null)
                        {
                            csv.WriteField(parameterCode);
                            continue;
                        }
                        var parameterInformationFromDss = dssModelInformation
                            .Output
                            .ResultParameters
                            .Where(n => n.Id == parameterCode)
                            .FirstOrDefault();
                        if (parameterInformationFromDss == null)
                        {
                            csv.WriteField(parameterCode);
                            continue;
                        }
                        csv.WriteField(parameterInformationFromDss.Title);
                    }
                    csv.NextRecord();

                    var locationResult = dssFullOutputAsObject.LocationResult.FirstOrDefault();
                    for (int i = 0; i < locationResult.Length; i++)
                    {
                        string formattedDate = FormatDate(dssFullOutputAsObject.TimeStart, i, isHourlyInterval);
                        csv.WriteField(formattedDate);
                        csv.WriteField(locationResult.WarningStatus[i].ToString());

                        if (locationResult.Data.Count() >= locationResult.Length)
                        {
                            List<double?> rowData = locationResult.Data[i];
                            foreach (double? value in rowData)
                            {
                                csv.WriteField(value);
                            }
                        }
                        csv.NextRecord();
                    }
                    writer.Flush();

                    var content = memoryStream.ToArray();
                    return GenericResponseBuilder.Success<byte[]>(content);
                }
            }
            catch (Exception ex)
            {
                logger.LogError(string.Format("Error in BLL - GetFieldCropPestDssDataAsCSVById. {0}", ex.Message), ex);
                String innerMessage = (ex.InnerException != null) ? ex.InnerException.Message : "";
                return GenericResponseBuilder.NoSuccess<byte[]>(null, $"{ex.Message} InnerException: {innerMessage}");
            }
        }

        private static string FormatDate(string timeStart, int increment, bool isHourlyInterval)
        {
            DateTime dateTime;
            if (isHourlyInterval)
            {
                dateTime = Convert.ToDateTime(timeStart).AddHours(increment);
                return dateTime.ToString("yyyy-MM-ddTHH:mm:ssZ");
            }
            else
            {
                dateTime = Convert.ToDateTime(timeStart).AddDays(increment);
                return dateTime.ToString("yyyy-MM-dd");
            }
        }

        private async Task AddExtraInformationToDss(IEnumerable<FieldDssResultDto> dssResultsToReturn, bool outOfSeason = false, List<Guid> listFarmsIdsAsOwner = null)
        {
            try
            {
                var listOfDss = await this.internalCommunicationProvider.GetAllListOfDssFromDssMicroservice();
                var eppoCodesData = await this.dataService.EppoCodes.GetEppoCodesAsync();
                var monitoringApi = JobStorage.Current.GetMonitoringApi();
                var listOfDisableDss = await this.dataService.DisabledDss.GetAllAsync();
                var farmIds = dssResultsToReturn.GroupBy(t => t.FarmId).Select(grp => grp.First().FarmId).ToList();
                List<FarmWithDssAvailableByLocation> listOfFarms = await GetDssModelAvailableFromFarmIds(farmIds);
                foreach (var dss in dssResultsToReturn)
                {
                    if (!listFarmsIdsAsOwner.Contains(dss.FarmId))
                    {
                        dss.IsOwner = false;
                    }
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

                            if (listOfDisableDss.Any(
                                d => d.DssId == dss.DssId
                                && d.DssVersion == dss.DssVersion
                                && d.DssModelId == dss.DssModelId
                                && d.DssModelVersion == dss.DssModelVersion))
                            {
                                AddDisableDssMessage(dss);
                                continue;
                            }

                            if (dssModelMatchDatabaseRecord.Output != null)
                            {
                                AddWarningMessages(dss, dssModelMatchDatabaseRecord);
                            }

                            if (!listOfFarms.Any(
                                f => f.FarmId == dss.FarmId
                                && f.DssModelsAvailable.Any(
                                    d => d.Id == dss.DssModelId)))
                            {
                                AddNotValidatedOnLocation(dss);
                            }

                            if (!outOfSeason) CheckIfDssOutOfSeason(dss);

                            // Remove Full result output
                            dss.DssFullResult = "";
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
                    else
                    {
                        // This statament is because old successful jobs might be removed from the DB
                        var fakeJobDetail = new DssTaskStatusDto();
                        fakeJobDetail.JobStatus = "Succeeded";
                        fakeJobDetail.DssId = dss.Id;
                        fakeJobDetail.Id = dss.DssTaskStatusDto.Id;
                        dss.DssTaskStatusDto = fakeJobDetail;
                    }
                }
            }
            catch (Exception ex)
            {
                logger.LogError(string.Format("Error in BLL - AddExtraInformationToDss. {0}. Full error: {1}", ex.Message, ex));
            }
        }

        private void CheckIfDssOutOfSeason(FieldDssResultDto dss)
        {
            var fullResult = JsonConvert.DeserializeObject<DssModelOutputInformation>(dss.DssFullResult);
            if (fullResult != null && fullResult.MessageType == 2 && fullResult.Message != null && !string.IsNullOrWhiteSpace(fullResult.Message))
            {
                dss.IsValid = false;
                dss.ResultMessageType = fullResult.MessageType;
                dss.ResultMessage = fullResult.Message;
                dss.WarningStatus = 0;
                return;
            }
            if (fullResult != null && fullResult.TimeEnd != null && DateTime.Parse(fullResult.TimeEnd) < DateTime.Today)
            {
                var dateTimeAsShortDate = DateTime.Parse(fullResult.TimeEnd).ToString("dd/MM/yyyy");
                dss.IsValid = false;
                dss.ResultMessageType = (int)DssOutputMessageTypeEnum.Info;
                dss.ResultMessage = this.jsonStringLocalizer["weather.end_of_season", dateTimeAsShortDate].ToString();
                dss.WarningStatus = 0;
                return;
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

        private async Task<FieldDssResultDetailedDto> CreateDetailedResultToReturn(FieldCropPestDss dss, int daysDataToReturn = 0)
        {
            var dataToReturn = this.mapper.Map<FieldDssResultDetailedDto>(dss);
            var dssInformation = await internalCommunicationProvider
                            .GetDssInformationFromDssMicroservice(dss.CropPestDss.DssId);

            if (dssInformation == null) return dataToReturn;
            dataToReturn.DssSource = string.Format("{0}, {1}",
                            dssInformation.DssOrganization.Name,
                            dssInformation.DssOrganization.Country);
            dataToReturn.DssVersion = dssInformation.Version;
            if (!string.IsNullOrEmpty(dssInformation.LogoUrl) && (!dssInformation.LogoUrl.StartsWith("http")))
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
            if (daysDataToReturn == 0)
            {
                if (int.Parse(dataToReturn.Interval) == 3600) locationResultData.Length = locationResultData.Length / 24;
                daysDataToReturn = locationResultData.Length;
            }
            else if (daysDataToReturn > 0 && daysDataToReturn < 7) daysDataToReturn = 7;

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
                var resultParameter = new ResultParameters
                {
                    Labels = labels,
                    Code = parameterCode
                };
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
            if (dssModelInformation.Output.ChartGroups == null) return dataToReturn;
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
                if (int.Parse(dataToReturn.Interval) == 3600)
                {
                    List<List<double?>> dataByDay;
                    List<int?> calculatedWarningStatusPerDay;
                    CalculateDailyDataFromHourly(dataToReturn, locationResultData, daysOutput, out dataByDay, out calculatedWarningStatusPerDay);

                    dataLastDays = dataByDay.TakeLast(daysOutput);
                    dataToReturn.ResultParametersLength = dataLastDays.Count();
                    dataToReturn.WarningStatusPerDay = calculatedWarningStatusPerDay.TakeLast(daysOutput).ToList();
                }
                else
                {
                    dataLastDays = locationResultData.Data.TakeLast(daysOutput);
                    dataToReturn.ResultParametersLength = dataLastDays.Count();
                    dataToReturn.WarningStatusPerDay = locationResultData.WarningStatus.TakeLast(daysOutput).ToList();
                }
            };
            return dataLastDays;
        }

        private static void CalculateDailyDataFromHourly(FieldDssResultDetailedDto dataToReturn, LocationResultDssOutput locationResultData, int daysOutput, out List<List<double?>> dataByDay, out List<int?> calculatedWarningStatusPerDay)
        {
            try
            {
                DateTime timeStart = DateTime.Parse(dataToReturn.OutputTimeStart);
                DateTime timeEnd = DateTime.Parse(dataToReturn.OutputTimeEnd);

                int index = locationResultData.Data.Count - 1;
                int lastIndex = locationResultData.Data.Count - ((daysOutput - 1) * 24 + timeEnd.Hour);

                var selectedData = locationResultData.Data.Skip(index - (timeEnd.Hour - 1)).Take(timeEnd.Hour);

                var selectedWarningStatus = locationResultData.WarningStatus.Skip(index - (timeEnd.Hour - 1)).Take(timeEnd.Hour);

                dataByDay = new List<List<double?>>();
                calculatedWarningStatusPerDay = new List<int?>();
                if (selectedData.Count() == 0) return;
                dataByDay.Add(GetMaxValueFromDoubleListOnList(selectedData));
                calculatedWarningStatusPerDay.Add(selectedWarningStatus.Max());

                var day = 1;
                int indexAfterLastDay = index - (timeEnd.Hour);
                while (indexAfterLastDay >= lastIndex)
                {
                    selectedData = locationResultData.Data.Skip(indexAfterLastDay - 23).Take(24);
                    selectedWarningStatus = locationResultData.WarningStatus.Skip(indexAfterLastDay - 23).Take(24);

                    dataByDay.Insert(0, GetMaxValueFromDoubleListOnList(selectedData));
                    calculatedWarningStatusPerDay.Insert(0, selectedWarningStatus.Max());

                    day += 1;
                    indexAfterLastDay -= 24;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private static List<double?> GetMaxValueFromDoubleListOnList(IEnumerable<List<double?>> data)
        {
            List<double?> maxValues = new List<double?>(data.FirstOrDefault().Count);
            for (int i = 0; i < maxValues.Capacity; i++)
            {
                maxValues.Add(null);
            }
            foreach (List<double?> dataItem in data)
            {
                for (int i = 0; i < dataItem.Count; i++)
                {
                    if (maxValues[i] == null || dataItem[i] > maxValues[i])
                    {
                        maxValues[i] = dataItem[i];
                    }
                }
            }
            return maxValues;
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

        private void AddWarningMessages(FieldDssResultBaseDto dss, DssModelInformation dssModelInformation)
        {
            dss.WarningExplanation = dssModelInformation.Output.ListWarningStatusInterpretation[dss.WarningStatus].Explanation;
            dss.WarningRecommendedAction = dssModelInformation.Output.ListWarningStatusInterpretation[dss.WarningStatus].RecommendedAction;
            dss.WarningStatusRepresentation = this.jsonStringLocalizer["dss.warning_status_representation",
                dss.WarningExplanation, dss.WarningRecommendedAction].ToString();
        }

        private static GeoJsonFeatureCollection CreateFarmsLocationGeoJson(IEnumerable<Farm> farmsFromUser)
        {
            var geoJson = new GeoJsonFeatureCollection();
            foreach (var farm in farmsFromUser)
            {
                AddFarmLocationToGeoJson(geoJson, farm);
            }
            return geoJson;
        }

        private static void AddFarmLocationToGeoJson(GeoJsonFeatureCollection geoJson, Farm farm)
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
                var jsonSchemaWithDate = DssDataHelper.AddDefaultDatesToDssJsonInput(dssInputUISchema.ToString());
                inputAsJsonObject = JObject.Parse(jsonSchemaWithDate.ToString());
                JObject userParametersAsJsonObject = JObject.Parse(dss.DssParameters.ToString());
                DssDataHelper.AddDefaultDssParametersToInputSchema(inputAsJsonObject, userParametersAsJsonObject);
                if (!displayInternalParameters)
                {
                    var dssModelInformation = await internalCommunicationProvider
                                            .GetDssModelInformationFromDssMicroservice(dss.CropPestDss.DssId, dss.CropPestDss.DssModelId);

                    var dssInternalParameters = dssModelInformation.Execution.InputSchemaCategories.Internal;
                    var dssSystemParameters = dssModelInformation.Execution.InputSchemaCategories.System;
                    dssInternalParameters.AddRange(dssSystemParameters);
                    if (dssInternalParameters.Count != 0)
                    {
                        DssDataHelper.HideInternalDssParametersFromInputSchema(inputAsJsonObject, dssInternalParameters);
                    }
                }
                bool hasFieldObservation = JsonHelper.HasProperty(inputAsJsonObject, "fieldObservation");
                bool userAlreadyHaveInputFieldObservation = dss.DssParameters.ToString().Contains("fieldObservations", StringComparison.OrdinalIgnoreCase);
                // remove field observations extra data
                if (hasFieldObservation)
                {
                    ChangeFieldObservationToOnlyTime(inputAsJsonObject);
                }
                if (userAlreadyHaveInputFieldObservation)
                {
                    RemoveExtraDataFieldObservation(inputAsJsonObject);
                }
            }
            return inputAsJsonObject;
        }

        private static void ChangeFieldObservationToOnlyTime(JObject jsonObject)
        {
            JObject newFieldObservation = new JObject(
                                    new JProperty("title", "Generic field observation information"),
                                    new JProperty("type", "object"),
                                    new JProperty("required", new JArray("time")),
                                    new JProperty("properties",
                                        new JObject(
                                            new JProperty("time",
                                                new JObject(
                                                    new JProperty("type", "string"),
                                                    new JProperty("format", "date-time"),
                                                    new JProperty("description", "The timestamp of the field observation. Format: \"yyyy-MM-dd\", e.g. 2020-04-09"),
                                                    new JProperty("title", "Time (yyyy-MM-dd)"),
                                                    new JProperty("default", DateTime.Today.ToString("yyyy-MM-dd"))
                                                )
                                            )
                                        )
                                    )
                                );
            JProperty fieldObservationProperty = JsonHelper.FindPropertyByName(jsonObject, "fieldObservation", "default");
            if (fieldObservationProperty != null)
            {
                fieldObservationProperty.Value = newFieldObservation;
            }
        }

        private static void RemoveExtraDataFieldObservation(JObject jsonObject)
        {
            var fieldObservationsPath = JsonHelper.GetPropertyPath(jsonObject, "fieldObservations");
            JToken fieldObservationsToken = jsonObject.SelectToken(fieldObservationsPath);

            if (fieldObservationsToken["default"] is JArray fieldObservationsArray)
            {
                foreach (JObject fieldObservation in fieldObservationsArray.Children<JObject>())
                {
                    JToken fieldObservationToken = fieldObservation.GetValue("fieldObservation");
                    if (fieldObservationToken is JObject fieldObservationObject)
                    {
                        fieldObservationObject.Remove("cropEPPOCode");
                        fieldObservationObject.Remove("pestEPPOCode");
                        fieldObservationObject.Remove("location");
                    }
                }
            }
        }

        private void AddNotValidatedOnLocation(FieldDssResultDto dss)
        {
            dss.IsValidInLocation = false;
            dss.ResultMessageType = (dss.ResultMessageType == null)
                ? (int)DssOutputMessageTypeEnum.Info
                : dss.ResultMessageType;
            dss.ResultMessage = (string.IsNullOrEmpty(dss.ResultMessage))
                ? this.jsonStringLocalizer["dss.dss_is_not_valid_location"].ToString()
                : string.Format("{0} \r\n {1}", dss.ResultMessage, this.jsonStringLocalizer["dss.dss_is_not_valid_location"].ToString());
        }

        private async Task<List<FarmWithDssAvailableByLocation>> GetDssModelAvailableFromFarmIds(List<Guid> farmIds)
        {
            var listOfFarms = new List<FarmWithDssAvailableByLocation>();
            var farms = await this.dataService.Farms.FindAllByConditionAsync(f => farmIds.Contains(f.Id));
            foreach (var farm in farms)
            {
                var farmGeoJson = new GeoJsonFeatureCollection();
                AddFarmLocationToGeoJson(farmGeoJson, farm);
                var listDssAvailable = await this.internalCommunicationProvider.GetListOfDssByLocationFromDssMicroservice(farmGeoJson, "ONTHEFLY");
                var listDssModelsAvailable = new List<DssModelInformation>();
                if (listDssAvailable != null && listDssAvailable.Count() != 0)
                {
                    listDssModelsAvailable = listDssAvailable.SelectMany(f => f.DssModelInformation).ToList();
                };
                var farmWithDss = new FarmWithDssAvailableByLocation()
                {
                    FarmId = farm.Id,
                    DssModelsAvailable = listDssModelsAvailable
                };

                listOfFarms.Add(farmWithDss);
            }
            return listOfFarms;
        }

        private void AddDisableDssMessage(FieldDssResultDto dss)
        {
            dss.IsValid = false;
            dss.ResultMessageType = (int)DssOutputMessageTypeEnum.Error;
            dss.ResultMessage = this.jsonStringLocalizer["dss.dss_is_disabled"].ToString();
            dss.WarningStatus = 0;
            dss.DssFullResult = "";
            dss.IsDisabled = true;
        }
    }
}