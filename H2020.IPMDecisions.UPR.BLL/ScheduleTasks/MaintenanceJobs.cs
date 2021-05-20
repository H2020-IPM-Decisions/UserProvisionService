using System;
using System.Threading.Tasks;
using AutoMapper;
using H2020.IPMDecisions.UPR.BLL.Helpers;
using H2020.IPMDecisions.UPR.BLL.Providers;
using H2020.IPMDecisions.UPR.Data.Core;
using Hangfire;
using Microsoft.AspNetCore.DataProtection;
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
        private readonly IMicroservicesInternalCommunicationHttpProvider internalCommunicationProvider;
        private readonly ILogger<DssRunningJobs> logger;
        private readonly IDataProtectionProvider dataProtectionProvider;
        private readonly IMapper mapper;
        private EncryptionHelper _encryption;
        public MaintenanceJobs(
            IDataService dataService,
            IMicroservicesInternalCommunicationHttpProvider internalCommunicationProvider,
            ILogger<DssRunningJobs> logger,
            IDataProtectionProvider dataProtectionProvider,
            IMapper mapper)
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
                var maxDaysResult = 7;
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