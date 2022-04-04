using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using H2020.IPMDecisions.UPR.BLL.Helpers;
using H2020.IPMDecisions.UPR.Core.Dtos;
using H2020.IPMDecisions.UPR.Core.Entities;
using H2020.IPMDecisions.UPR.Core.Models;
using Hangfire;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

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

        public async Task<GenericResponse<IEnumerable<FieldDssResultDto>>> GetAllDssResults(Guid userId)
        {
            try
            {
                var dssResults = await this.dataService.DssResult.GetAllDssResults(userId);
                var dssResultsToReturn = this.mapper.Map<IEnumerable<FieldDssResultDto>>(dssResults);

                if (dssResultsToReturn != null && dssResultsToReturn.Count() != 0)
                {
                    await AddExtraInformationToDss(dssResultsToReturn);
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

        public async Task<GenericResponse<string>> GetFieldCropPestDssParametersById(Guid id, Guid userId)
        {
            try
            {
                var dss = await this.dataService.FieldCropPestDsses.FindByIdAsync(id);
                if (dss == null) return GenericResponseBuilder.NotFound<string>();

                var dssUserId = dss.FieldCropPest.FieldCrop.Field.Farm.UserFarms.FirstOrDefault().UserId;
                if (userId != dssUserId) return GenericResponseBuilder.NotFound<string>();

                var dssInputUISchema = await internalCommunicationProvider
                                       .GetDssModelInputSchemaMicroservice(dss.CropPestDss.DssId, dss.CropPestDss.DssModelId);
                JObject inputAsJsonObject = null;
                if (dssInputUISchema != null)
                {
                    inputAsJsonObject = JObject.Parse(dssInputUISchema.ToString());
                    JObject userParametersAsJsonObject = JObject.Parse(dss.DssParameters.ToString());
                    DssDataHelper.AddDefaultDssParametersToInputSchema(inputAsJsonObject, userParametersAsJsonObject);
                }
                return GenericResponseBuilder.Success<string>(inputAsJsonObject.ToString());
            }
            catch (Exception ex)
            {
                logger.LogError(string.Format("Error in BLL - GetFieldCropPestDssParametersById. {0}", ex.Message), ex);
                String innerMessage = (ex.InnerException != null) ? ex.InnerException.Message : "";
                return GenericResponseBuilder.NoSuccess<string>(null, $"{ex.Message} InnerException: {innerMessage}");
            }
        }

        private async Task AddExtraInformationToDss(IEnumerable<FieldDssResultDto> dssResultsToReturn)
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
                        .FirstOrDefault()
                        .DssModelInformation
                        .Where(dm => dm.Id == dss.DssModelId)
                         .FirstOrDefault();

                    if (dssOnListMatchDatabaseRecord == null)
                    {
                        dss.DssDescription = this.jsonStringLocalizer["dss.model_missing_metadata",
                            dss.DssId,
                            dss.DssVersion,
                            dss.DssModelId,
                            dss.DssModelVersion].ToString();
                    }
                    else
                    {
                        dss.DssDescription = CreateDssDescription(dssOnListMatchDatabaseRecord.Description);
                        dss.ValidatedSpatialCountries = dssOnListMatchDatabaseRecord.ValidSpatial.Countries;
                        if (dssOnListMatchDatabaseRecord.Output != null)
                        {
                            AddWarningMessages(dss, dssOnListMatchDatabaseRecord);
                        }
                    }
                }

                var eppoCodeLanguages = EppoCodesHelper.GetCropPestEppoCodesNames(eppoCodesData, dss.CropEppoCode, dss.PestEppoCode);
                dss.CropLanguages = eppoCodeLanguages.CropLanguages;
                dss.PestLanguages = eppoCodeLanguages.PestLanguages;

                if (string.IsNullOrEmpty(dss.DssTaskStatusDto.Id)) continue;
                var jobDetail = monitoringApi.JobDetails(dss.DssTaskStatusDto.Id);
                if (jobDetail != null)
                {
                    dss.DssTaskStatusDto = CreateDssStatusFromJobDetail(dss.Id, dss.DssTaskStatusDto.Id, jobDetail);
                }
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
                            .GetDssModelInformationFromDssMicroservice(dss.CropPestDss.DssId, dss.CropPestDss.DssModelId);

            if (dssInformation == null) return dataToReturn;
            AddDssBasicData(dataToReturn, dssInformation);
            await AddDssCropPestNames(dataToReturn);

            if (!dataToReturn.IsValid || dataToReturn.DssExecutionType.ToLower() == "link") return dataToReturn;
            DssModelOutputInformation dssFullOutputAsObject = AddDssFullResultData(dataToReturn);

            var locationResultData = dssFullOutputAsObject.LocationResult.FirstOrDefault();
            int maxDaysOutput = int.Parse(this.config["AppConfiguration:MaxDaysAllowedForDssOutputData"]);
            IEnumerable<List<double?>> dataLastDays = SelectDssLastResultsData(dataToReturn, locationResultData, daysDataToReturn, maxDaysOutput);
            List<string> labels = CreateResultParametersLabels(dataToReturn.OutputTimeEnd, dataLastDays.Count());
            dataToReturn.WarningStatusLabels = labels;

            for (int i = 0; i < dssFullOutputAsObject.ResultParameters.Count; i++)
            {
                var parameterCode = dssFullOutputAsObject.ResultParameters[i];
                var resultParameter = new ResultParameters();
                resultParameter.Labels = labels;

                resultParameter.Code = parameterCode;
                var parameterInformationFromDss = dssInformation
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
            foreach (var group in dssInformation.Output.ChartGroups)
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

        // This can be implemented by AutoMapper
        private static void AddDssBasicData(FieldDssResultDetailedDto dataToReturn, DssModelInformation dssInformation)
        {
            dataToReturn.DssTypeOfDecision = dssInformation.TypeOfDecision;
            dataToReturn.DssTypeOfOutput = dssInformation.TypeOfOutput;
            dataToReturn.DssDescription = CreateDssDescription(dssInformation.Description);
            dataToReturn.DssEndPoint = dssInformation.DescriptionUrl;
            dataToReturn.ValidatedSpatialCountries = dssInformation.ValidSpatial.Countries;

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

        private static string CreateDssDescription(DssDescription description)
        {
            var dssDescriptionJoined = "";
            if (!string.IsNullOrEmpty(description.Other))
                dssDescriptionJoined = string.Format("{0}Other: {1}. ", dssDescriptionJoined, description.Other);

            if (!string.IsNullOrEmpty(description.CreatedBy))
                dssDescriptionJoined = string.Format("{0}Created by: {1}. ", dssDescriptionJoined, description.CreatedBy);

            if (!string.IsNullOrEmpty(description.Age))
                dssDescriptionJoined = string.Format("{0}Age: {1}. ", dssDescriptionJoined, description.Age);

            if (!string.IsNullOrEmpty(description.Assumptions))
                dssDescriptionJoined = string.Format("{0}Assumptions: {1}. ", dssDescriptionJoined, description.Assumptions);

            if (!string.IsNullOrEmpty(description.PeerReview))
                dssDescriptionJoined = string.Format("{0}Peer review: {1}. ", dssDescriptionJoined, description.PeerReview);

            return dssDescriptionJoined;
        }

        private static void AddWarningMessages(FieldDssResultBaseDto dss, DssModelInformation dssModelInformation)
        {
            dss.WarningStatusRepresentation = dssModelInformation.Output.ListWarningStatusInterpretation[dss.WarningStatus].Explanation;
            dss.WarningMessage = dssModelInformation.Output.ListWarningStatusInterpretation[dss.WarningStatus].RecommendedAction;
        }
    }
}