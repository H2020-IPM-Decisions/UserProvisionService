using System;
using Hangfire;
using Microsoft.Extensions.Configuration;

namespace H2020.IPMDecisions.UPR.BLL.ScheduleTasks
{
    public class HangfireJobScheduler
    {
        public static void HangfireScheduleJobs(IConfiguration configuration)
        {
            RecurringJob.AddOrUpdate<DssRunningJobs>(nameof(DssRunningJobs.ExecuteOnTheFlyDss),
                job => job.ExecuteOnTheFlyDss(JobCancellationToken.Null),
                Cron.Daily(int.Parse(configuration["AppConfiguration:HourToRunNightSchedule"])), TimeZoneInfo.Utc);

            RecurringJob.AddOrUpdate<MaintenanceJobs>(nameof(MaintenanceJobs.DeleteOldDssResults),
                job => job.DeleteOldDssResults(JobCancellationToken.Null),
                Cron.Daily(23, 30), TimeZoneInfo.Utc);

            RecurringJob.AddOrUpdate<DssRunningJobs>(nameof(DssRunningJobs.ExecuteDssWithErrors),
                job => job.ExecuteDssWithErrors(JobCancellationToken.Null),
                "0 */6 * * *", TimeZoneInfo.Utc);
        }
    }
}