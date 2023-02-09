using System;
using System.Threading.Tasks;
using H2020.IPMDecisions.UPR.Data.Core;
using Hangfire;
using Microsoft.Extensions.Logging;

namespace H2020.IPMDecisions.UPR.BLL.ScheduleTasks
{
    public interface IMaintenanceJobs
    {
        void DeleteOldDssResults(IJobCancellationToken token);
    }

    public class MaintenanceJobs : IMaintenanceJobs
    {
        private readonly IDataService dataService;
        private readonly ILogger<MaintenanceJobs> logger;
        public MaintenanceJobs(
            IDataService dataService,
            ILogger<MaintenanceJobs> logger)
        {
            this.dataService = dataService
                ?? throw new ArgumentNullException(nameof(dataService));
            this.logger = logger
                ?? throw new ArgumentNullException(nameof(logger));
        }

        [Queue("deleteoldresults_schedule")]
        public void DeleteOldDssResults(IJobCancellationToken token)
        {
            try
            {
                token.ThrowIfCancellationRequested();
                var maxDaysResult = 2;
                Task.Run(() => DeleteResultOlderThan(maxDaysResult)).Wait();
            }
            catch (Exception ex)
            {
                logger.LogError(string.Format("Error in BLL - Error executing schedule to delete old DSS results. {0}", ex.Message));
            }
        }

        private async Task DeleteResultOlderThan(int days)
        {
            await this.dataService
                .FieldCropPestDsses
                .DeleteDssResultsByCondition(f => f.CreationDate < DateTime.Now.AddDays(-days));
            await this.dataService.CompleteAsync();
        }
    }
}