using System;
using Hangfire;

namespace H2020.IPMDecisions.UPR.BLL.ScheduleTasks
{
    public class HangfireJobScheduler
    {
        public static void HangfireScheduleJobs()
        {
            RecurringJob.AddOrUpdate<DssRunningJobs>(nameof(DssRunningJobs.ExecuteOnTheFlyDss),
                job => job.ExecuteOnTheFlyDss(JobCancellationToken.Null),
                Cron.Daily(), TimeZoneInfo.Utc);

            RecurringJob.AddOrUpdate<MaintenanceJobs>(nameof(MaintenanceJobs.DeleteOldDssResults),
                job => job.DeleteOldDssResults(JobCancellationToken.Null),
                Cron.Daily(23, 30), TimeZoneInfo.Utc);
        }
    }
}