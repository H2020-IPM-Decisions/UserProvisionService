using System;
using H2020.IPMDecisions.UPR.Data.Core;
using Hangfire;

namespace H2020.IPMDecisions.UPR.BLL.ScheduleTasks
{
    public interface IHangfireQueueJobs
    {
        string AddDssOnOnTheFlyQueue(Guid id);
    }

    public class HangfireQueueJobs : IHangfireQueueJobs
    {
        private readonly IDataService dataService;

        public HangfireQueueJobs(
            IDataService dataService)
        {
            this.dataService = dataService
                ?? throw new ArgumentNullException(nameof(dataService));
        }

        public string AddDssOnOnTheFlyQueue(Guid id)
        {
            return BackgroundJob.Enqueue<DssRunningJobs>(
               job => job.QueueOnTheFlyDss(JobCancellationToken.Null, id));
        }
    }
}