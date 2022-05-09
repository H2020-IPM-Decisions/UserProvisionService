using System;
using System.Linq;
using System.Threading.Tasks;
using H2020.IPMDecisions.UPR.Core.Dtos;
using H2020.IPMDecisions.UPR.Core.Entities;
using H2020.IPMDecisions.UPR.Core.Models;
using Hangfire;
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
                    DssOriginalParameters = await GenerateUserDssParameters(dss),
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
                var validationErrormessages = await ValidateNewDssParameters(dss.CropPestDss, fieldCropPestDssForUpdateDto.DssParameters);
                if (validationErrormessages.Count > 0)
                {
                    var errorMessageToReturn = string.Join(" ", validationErrormessages);
                    return GenericResponseBuilder.NoSuccess<DssTaskStatusDto>(null, errorMessageToReturn);
                }

                var jobId = this.queueJobs.AddDssOnTheFlyQueue(id, true);
                var monitoringApi = JobStorage.Current.GetMonitoringApi();
                var jobDetail = monitoringApi.JobDetails(jobId);
                if (jobDetail == null) return GenericResponseBuilder.NotFound<DssTaskStatusDto>();
                DssTaskStatusDto dataToReturn = CreateDssStatusFromJobDetail(id, jobId, jobDetail);

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
                        // ToDo change days
                        dssResult = await CreateDetailedResultToReturn(newDss, daysDataToReturn);
                    }
                }
                var dataTorReturn = new AdaptationTaskDssResult()
                {
                    DssTaskStatusDto = taskStatus,
                    DssDetailedResult = dssResult
                };

                return GenericResponseBuilder.Success<AdaptationTaskDssResult>(dataTorReturn);
            }
            catch (Exception ex)
            {
                logger.LogError(string.Format("Error in BLL - GetTaskStatusById. {0}", ex.Message));
                String innerMessage = (ex.InnerException != null) ? ex.InnerException.Message : "";
                return GenericResponseBuilder.NoSuccess<AdaptationTaskDssResult>(null, $"{ex.Message} InnerException: {innerMessage}");
            }
        }
    }
}