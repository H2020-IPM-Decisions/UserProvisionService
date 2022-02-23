using System;
using H2020.IPMDecisions.UPR.Data.Core;
using Hangfire;

namespace H2020.IPMDecisions.UPR.BLL.ScheduleTasks
{
    public interface IHangfireQueueJobs
    {
        string AddDssOnTheFlyQueue(Guid id);
        string ScheduleDssOnTheFlyQueue(Guid id, int hours);
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

        public string AddDssOnTheFlyQueue(Guid id)
        {
            return BackgroundJob.Enqueue<DssRunningJobs>(
               job => job.QueueOnTheFlyDss(JobCancellationToken.Null, id));
        }

        public string ScheduleDssOnTheFlyQueue(Guid id, int minutes)
        {
            return BackgroundJob.Schedule<DssRunningJobs>(
                job => job.QueueOnTheFlyDss(JobCancellationToken.Null, id), TimeSpan.FromMinutes(minutes));
        }
    }
}