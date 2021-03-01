using System;
using Hangfire;

namespace H2020.IPMDecisions.UPR.BLL.ScheduleTasks
{
    public class HangfireJobScheduler
    {
        public static void ScheduleRecurringJobs()
        {
            RecurringJob.RemoveIfExists(nameof(RunDssOnDatabase));
            RecurringJob.AddOrUpdate<RunDssOnDatabase>(nameof(RunDssOnDatabase),
                job => job.Execute(JobCancellationToken.Null),
                Cron.Minutely, TimeZoneInfo.Local);
        }
    }
}