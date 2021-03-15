using System;
using Hangfire;

namespace H2020.IPMDecisions.UPR.BLL.ScheduleTasks
{
    public class HangfireJobScheduler
    {
        public static void ScheduleRecurringJobs()
        {
            RecurringJob.RemoveIfExists(nameof(RunDssOnDatabase.ExecuteOnTheFlyDss));
            RecurringJob.AddOrUpdate<RunDssOnDatabase>(nameof(RunDssOnDatabase.ExecuteOnTheFlyDss),
                job => job.ExecuteOnTheFlyDss(JobCancellationToken.Null),
                Cron.Daily(), TimeZoneInfo.Utc);
        }
    }
}