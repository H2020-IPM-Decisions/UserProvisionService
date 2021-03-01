using System;
using Hangfire;

namespace H2020.IPMDecisions.UPR.BLL.ScheduleTaks
{
    public class RunDssOnDatabase
    {
        private readonly IRecurringJobManager recurringJobManager;

        public RunDssOnDatabase(IRecurringJobManager recurringJobManager)
        {
            this.recurringJobManager = recurringJobManager
                ?? throw new ArgumentNullException(nameof(recurringJobManager));
        }

        public void Execute()
        {
            recurringJobManager.AddOrUpdate(
                "test",
                () => Console.WriteLine("I'm a recurring job!"),
                Cron.Minutely);
        }
    }
}