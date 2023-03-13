using System.Threading.Tasks;
using H2020.IPMDecisions.UPR.Core.Dtos;
using Hangfire;
using System;
using Microsoft.Extensions.Logging;
using H2020.IPMDecisions.UPR.Core.Models;
using System.Linq;
using Hangfire.Storage.Monitoring;

namespace H2020.IPMDecisions.UPR.BLL
{
    public partial class BusinessLogic : IBusinessLogic
    {
        public async Task<GenericResponse<DssTaskStatusDto>> GetLatestTaskStatusByDssId(Guid dssId, Guid userId)
        {
            try
            {
                var dss = await this.dataService.FieldCropPestDsses.FindByIdAsync(dssId);
                if (dss == null) return GenericResponseBuilder.Unauthorized<DssTaskStatusDto>();

                if (!dss.FieldCropPest.FieldCrop.Field.Farm.UserFarms.Any(u => u.UserId == userId))
                {
                    return GenericResponseBuilder.Unauthorized<DssTaskStatusDto>();
                }

                var monitoringApi = JobStorage.Current.GetMonitoringApi();
                var jobDetail = monitoringApi.JobDetails(dss.LastJobId);

                if (jobDetail == null) return GenericResponseBuilder.Unauthorized<DssTaskStatusDto>();

                DssTaskStatusDto dataToReturn = CreateDssStatusFromJobDetail(dssId, dss.LastJobId, jobDetail);
                return GenericResponseBuilder.Success<DssTaskStatusDto>(dataToReturn);
            }
            catch (Exception ex)
            {
                logger.LogError(string.Format("Error in BLL - GetTaskStatusById. {0}", ex.Message));
                String innerMessage = (ex.InnerException != null) ? ex.InnerException.Message : "";
                return GenericResponseBuilder.NoSuccess<DssTaskStatusDto>(null, $"{ex.Message} InnerException: {innerMessage}");
            }
        }

        private DssTaskStatusDto CreateDssStatusFromJobDetail(Guid dssId, string taskId, JobDetailsDto jobDetail)
        {
            var lastStatus = jobDetail.History.FirstOrDefault();

            var dataToReturn = this.mapper.Map<DssTaskStatusDto>(lastStatus);
            dataToReturn.DssId = dssId;
            dataToReturn.Id = taskId;
            if (string.IsNullOrEmpty(dataToReturn.JobStatus))
            {
                dataToReturn.JobStatus = "Succeeded";
            }
            else if (dataToReturn.JobStatus.ToLower() == "scheduled")
            {
                dataToReturn.ScheduleTime = new DateTime(1970, 1, 1, 0, 0, 0)
                                    .AddMilliseconds(Convert.ToDouble(lastStatus.Data["EnqueueAt"])).ToLocalTime();
            }

            return dataToReturn;
        }
    }
}