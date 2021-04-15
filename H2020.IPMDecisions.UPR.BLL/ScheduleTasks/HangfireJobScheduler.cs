using System;
using Hangfire;

namespace H2020.IPMDecisions.UPR.BLL.ScheduleTasks
{
    public class HangfireJobScheduler
    {
        public static void HangfireScheduleJobs()
        {
            RecurringJob.RemoveIfExists(nameof(DssRunningJobs.ExecuteOnTheFlyDss));
            RecurringJob.AddOrUpdate<DssRunningJobs>(nameof(DssRunningJobs.ExecuteOnTheFlyDss),
                job => job.ExecuteOnTheFlyDss(JobCancellationToken.Null),
                Cron.Daily(), TimeZoneInfo.Utc);
        }
    }
}