using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using H2020.IPMDecisions.UPR.BLL.Helpers;
using H2020.IPMDecisions.UPR.Core.Dtos;
using H2020.IPMDecisions.UPR.Core.Entities;
using H2020.IPMDecisions.UPR.Core.Models;
using Hangfire;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace H2020.IPMDecisions.UPR.BLL
{
    public partial class BusinessLogic : IBusinessLogic
    {
        public async Task<GenericResponse<AdaptationDashboardDto>> GetAdaptationDataById(Guid id, Guid userId, int daysDataToReturn = 7)
        {
            try
            {
                var dss = await this.dataService.FieldCropPestDsses.FindByIdAsync(id);
                if (dss == null) return GenericResponseBuilder.NotFound<AdaptationDashboardDto>();

                var dssUserId = dss.FieldCropPest.FieldCrop.Field.Farm.UserFarms.FirstOrDefault().UserId;
                if (userId != dssUserId) return GenericResponseBuilder.NotFound<AdaptationDashboardDto>();

                var dataToReturn = new AdaptationDashboardDto()
                {
                    DssOriginalParameters = await GenerateUserDssParameters(dss, displayInternalParameters: true),
                    DssOriginalResult = await CreateDetailedResultToReturn(dss, daysDataToReturn)
                };

                return GenericResponseBuilder.Success<AdaptationDashboardDto>(dataToReturn);
            }
            catch (Exception ex)
            {
                logger.LogError(string.Format("Error in BLL - GetFieldCropPestDssParametersById. {0}", ex.Message), ex);
                String innerMessage = (ex.InnerException != null) ? ex.InnerException.Message : "";
                return GenericResponseBuilder.NoSuccess<AdaptationDashboardDto>(null, $"{ex.Message} InnerException: {innerMessage}");
            }
        }

        public async Task<GenericResponse<DssTaskStatusDto>> AddTaskToRunFieldCropPestDssById(Guid id, Guid userId, FieldCropPestDssForUpdateDto fieldCropPestDssForUpdateDto)
        {
            try
            {
                var dss = await this.dataService.FieldCropPestDsses.FindByIdAsync(id);
                if (dss == null) return GenericResponseBuilder.NotFound<DssTaskStatusDto>();

                var dssUserId = dss.FieldCropPest.FieldCrop.Field.Farm.UserFarms.FirstOrDefault().UserId;
                if (userId != dssUserId) return GenericResponseBuilder.NotFound<DssTaskStatusDto>();

                if (dss.CropPestDss.DssExecutionType.ToLower() != "onthefly")
                {
                    GenericResponseBuilder.NoSuccess<DssTaskStatusDto>(null, this.jsonStringLocalizer["dss_process.not_on_the_fly"].ToString());
                }

                // Validate DSS parameters on server side
                var validationErrorMessages = await ValidateNewDssParameters(dss.CropPestDss, fieldCropPestDssForUpdateDto.DssParameters);
                if (validationErrorMessages.Count > 0)
                {
                    var errorMessageToReturn = string.Join(" ", validationErrorMessages);
                    return GenericResponseBuilder.NoSuccess<DssTaskStatusDto>(null, errorMessageToReturn);
                }

                var dataToReturn = CreateTaskStatusDto(id, fieldCropPestDssForUpdateDto.DssParameters);
                if (dataToReturn == null) return GenericResponseBuilder.NotFound<DssTaskStatusDto>();
                return GenericResponseBuilder.Success<DssTaskStatusDto>(dataToReturn);
            }
            catch (Exception ex)
            {
                logger.LogError(string.Format("Error in BLL - UpdateFieldCropPestDssById. {0}", ex.Message), ex);
                String innerMessage = (ex.InnerException != null) ? ex.InnerException.Message : "";
                return GenericResponseBuilder.NoSuccess<DssTaskStatusDto>(null, $"{ex.Message} InnerException: {innerMessage}");
            }
        }

        public async Task<GenericResponse<AdaptationTaskDssResult>> GetDssResultFromTaskById(Guid dssId, string taskId, Guid userId, int daysDataToReturn = 7)
        {
            try
            {
                var dss = await this.dataService.FieldCropPestDsses.FindByIdAsync(dssId);
                if (dss == null) return GenericResponseBuilder.Unauthorized<AdaptationTaskDssResult>();

                if (!dss.FieldCropPest.FieldCrop.Field.Farm.UserFarms.Any(u => u.UserId == userId))
                {
                    return GenericResponseBuilder.Unauthorized<AdaptationTaskDssResult>();
                }

                var monitoringApi = JobStorage.Current.GetMonitoringApi();
                var jobDetail = monitoringApi.JobDetails(taskId);

                if (jobDetail == null) return GenericResponseBuilder.Unauthorized<AdaptationTaskDssResult>();
                if (Guid.Parse(jobDetail.Job.Args[1].ToString()) != dssId) return GenericResponseBuilder.Unauthorized<AdaptationTaskDssResult>();

                DssTaskStatusDto taskStatus = CreateDssStatusFromJobDetail(dssId, taskId, jobDetail);
                var dssResult = new FieldDssResultDetailedDto();
                if (taskStatus.JobStatus.ToLower() == "succeeded")
                {
                    var cacheKey = string.Format("InMemoryDssResult_{0}", taskStatus.Id);
                    if (memoryCache.TryGetValue(cacheKey, out object value))
                    {
                        var valueAsDssResult = (FieldDssResult)value;
                        var newDss = dss;
                        newDss.FieldDssResults.Clear();
                        newDss.FieldDssResults.Add(valueAsDssResult);
                        dssResult = await CreateDetailedResultToReturn(newDss, daysDataToReturn);
                    }
                }
                var dataToReturn = new AdaptationTaskDssResult()
                {
                    DssTaskStatusDto = taskStatus,
                    DssDetailedResult = dssResult
                };

                return GenericResponseBuilder.Success<AdaptationTaskDssResult>(dataToReturn);
            }
            catch (Exception ex)
            {
                logger.LogError(string.Format("Error in BLL - GetTaskStatusById. {0}", ex.Message));
                String innerMessage = (ex.InnerException != null) ? ex.InnerException.Message : "";
                return GenericResponseBuilder.NoSuccess<AdaptationTaskDssResult>(null, $"{ex.Message} InnerException: {innerMessage}");
            }
        }

        public async Task<GenericResponse<List<DssHistoricalDataTask>>> PrepareDssToRunFieldCropPestDssHistoricalDataById(Guid id, Guid userId, FieldCropPestDssForHistoricalDataDto fieldCropPestDssForUpdateDto)
        {
            try
            {
                var dss = await this.dataService.FieldCropPestDsses.FindByIdAsync(id);
                if (dss == null) return GenericResponseBuilder.NotFound<List<DssHistoricalDataTask>>();

                var dssUserId = dss.FieldCropPest.FieldCrop.Field.Farm.UserFarms.FirstOrDefault().UserId;
                if (userId != dssUserId) return GenericResponseBuilder.NotFound<List<DssHistoricalDataTask>>();

                if (dss.CropPestDss.DssExecutionType.ToLower() != "onthefly")
                {
                    GenericResponseBuilder.NoSuccess<DssHistoricalDataTask>(null, this.jsonStringLocalizer["dss_process.not_on_the_fly"].ToString());
                }

                var dssParametersToRun = fieldCropPestDssForUpdateDto.DssParameters;
                if (string.IsNullOrEmpty(dssParametersToRun))
                {
                    dssParametersToRun = dss.DssParameters;
                }
                else
                {
                    var validationErrorMessages = await ValidateNewDssParameters(dss.CropPestDss, dssParametersToRun);
                    if (validationErrorMessages.Count > 0)
                    {
                        var errorMessageToReturn = string.Join(" ", validationErrorMessages);
                        return GenericResponseBuilder.NoSuccess<List<DssHistoricalDataTask>>(null, errorMessageToReturn);
                    }
                }
                var listOfJobs = new List<DssHistoricalDataTask>();
                DssTaskStatusDto currentJobTask = CreateTaskStatusDto(id, dssParametersToRun);
                var currentDateJob = new DssHistoricalDataTask()
                {
                    TaskType = "Current",
                    TaskStatusDto = currentJobTask
                };

                // Remove one year from dates
                var dssModelInformation = await internalCommunicationProvider
                                        .GetDssModelInformationFromDssMicroservice(dss.CropPestDss.DssId, dss.CropPestDss.DssModelId);
                var historicalParameters = DssDataHelper.PrepareDssParametersForHistoricalYear(dssModelInformation.Input, dssParametersToRun);
                DssTaskStatusDto historicalJobTask = CreateTaskStatusDto(id, historicalParameters);
                var historicalDataJob = new DssHistoricalDataTask()
                {
                    TaskType = "Historical",
                    TaskStatusDto = historicalJobTask
                };
                listOfJobs.Add(currentDateJob);
                listOfJobs.Add(historicalDataJob);

                return GenericResponseBuilder.Success<List<DssHistoricalDataTask>>(listOfJobs);
            }
            catch (Exception ex)
            {
                logger.LogError(string.Format("Error in BLL - UpdateFieldCropPestDssById. {0}", ex.Message), ex);
                String innerMessage = (ex.InnerException != null) ? ex.InnerException.Message : "";
                return GenericResponseBuilder.NoSuccess<List<DssHistoricalDataTask>>(null, $"{ex.Message} InnerException: {innerMessage}");
            }
        }

        public async Task<GenericResponse<IDictionary<string, object>>> SaveAdaptationDss(Guid id, Guid userId, FieldCropPestDssForAdaptationDto fieldCropPestDssForAdaptationDto, HttpContext httpContext)
        {
            try
            {
                var dss = await this.dataService.FieldCropPestDsses.FindByIdAsync(id);
                if (dss == null) return GenericResponseBuilder.NotFound<IDictionary<string, object>>();

                var dssUserId = dss.FieldCropPest.FieldCrop.Field.Farm.UserFarms.FirstOrDefault().UserId;
                if (userId != dssUserId) return GenericResponseBuilder.NotFound<IDictionary<string, object>>();
                var validationErrorMessages = await ValidateNewDssParameters(dss.CropPestDss, fieldCropPestDssForAdaptationDto.DssParameters);

                var dataToReturn = new Dictionary<string, object>();
                if (validationErrorMessages.Count > 0)
                {
                    var errorMessageToReturn = string.Join(" ", validationErrorMessages);
                    return GenericResponseBuilder.NoSuccess<IDictionary<string, object>>(dataToReturn, errorMessageToReturn);
                }
                var existingCombination = dss.FieldCropPest.FieldCropPestDsses
                    .Where(fcpd =>
                        fcpd.CropPestDssId == dss.CropPestDssId
                        && fcpd.FieldCropPestId == dss.FieldCropPestId
                        && fcpd.IsCustomDss == true
                        && fcpd.CustomName == fieldCropPestDssForAdaptationDto.Name)
                    .FirstOrDefault();

                if (existingCombination != null)
                {
                    var errorMessage = this.jsonStringLocalizer["dss.adaptation_duplicated",
                        dss.CropPestDss.DssModelName,
                        fieldCropPestDssForAdaptationDto.Name].ToString();
                    dataToReturn.Add("warnings", errorMessage);
                    httpContext.Response.Headers.Add("warning", "Warning");
                    httpContext.Response.Headers.Add("warn-text", this.jsonStringLocalizer["dss.warning_header"].ToString());
                    return GenericResponseBuilder.Duplicated<IDictionary<string, object>>(errorMessage, dataToReturn);
                }

                var newDss = this.mapper
                    .Map<FieldCropPestDss>(fieldCropPestDssForAdaptationDto, opt =>
                    {
                        opt.Items["CropPestDssId"] = dss.CropPestDssId;
                        opt.Items["FieldCropPestId"] = dss.FieldCropPestId;
                    });

                this.dataService.FieldCropPestDsses.Create(newDss);
                await this.dataService.CompleteAsync();

                var fieldCropPestDssToReturn = this.mapper.Map<FieldCropPestDssDto>(newDss);
                if (newDss.CropPestDss.DssExecutionType.ToLower() == "onthefly")
                {
                    var jobId = this.queueJobs.AddDssOnTheFlyQueue(newDss.Id);
                    fieldCropPestDssToReturn.DssTask.Id = jobId;
                    newDss.LastJobId = jobId;
                }
                await this.dataService.CompleteAsync();
                dataToReturn.Add("value", fieldCropPestDssToReturn);
                return GenericResponseBuilder.Success<IDictionary<string, object>>(dataToReturn);
            }
            catch (Exception ex)
            {
                logger.LogError(string.Format("Error in BLL - SaveAdaptationDss. {0}", ex.Message), ex);
                String innerMessage = (ex.InnerException != null) ? ex.InnerException.Message : "";
                return GenericResponseBuilder.NoSuccess<IDictionary<string, object>>(null, $"{ex.Message} InnerException: {innerMessage}");
            }
        }

        private DssTaskStatusDto CreateTaskStatusDto(Guid dssId, string dssParameters)
        {
            var jobId = this.queueJobs.RunDssOnMemory(dssId, dssParameters);
            var monitoringApi = JobStorage.Current.GetMonitoringApi();
            var jobDetail = monitoringApi.JobDetails(jobId);
            if (jobDetail == null) return null;
            return CreateDssStatusFromJobDetail(dssId, jobId, jobDetail);
        }
    }
}