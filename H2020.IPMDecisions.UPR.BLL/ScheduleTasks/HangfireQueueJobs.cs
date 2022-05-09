using System;
using H2020.IPMDecisions.UPR.Data.Core;
using Hangfire;

namespace H2020.IPMDecisions.UPR.BLL.ScheduleTasks
{
    public interface IHangfireQueueJobs
    {
        string AddDssOnTheFlyQueue(Guid id, bool inMemory = false);
        string ScheduleDssOnTheFlyQueue(Guid id, double minutes);
        string ScheduleDssOnTheFlyQueueSeconds(Guid id, int second);
        string AddFarmLocationToWeatherQueue(string weatherStringParametersUrl);
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

        public string AddDssOnTheFlyQueue(Guid id, bool inMemory = false)
        {
            return BackgroundJob.Enqueue<DssRunningJobs>(
               job => job.QueueOnTheFlyDss(JobCancellationToken.Null, id, null, inMemory));
        }

        public string ScheduleDssOnTheFlyQueue(Guid id, double minutes)
        {
            return BackgroundJob.Schedule<DssRunningJobs>(
                job => job.QueueOnTheFlyDss(JobCancellationToken.Null, id, null, false), TimeSpan.FromMinutes(minutes));
        }

        public string ScheduleDssOnTheFlyQueueSeconds(Guid id, int seconds)
        {
            return BackgroundJob.Schedule<DssRunningJobs>(
                job => job.QueueOnTheFlyDss(JobCancellationToken.Null, id, null, false), TimeSpan.FromSeconds(seconds));
        }

        public string AddFarmLocationToWeatherQueue(string weatherStringParametersUrl)
        {
            return BackgroundJob.Enqueue<DssRunningJobs>(
               job => job.QueueWeatherToAmalgamationService(JobCancellationToken.Null, weatherStringParametersUrl));
        }
    }
}