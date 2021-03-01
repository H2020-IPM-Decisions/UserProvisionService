using System;
using System.Threading.Tasks;
using H2020.IPMDecisions.UPR.Data.Core;
using Hangfire;

namespace H2020.IPMDecisions.UPR.BLL.ScheduleTasks
{
    public interface IRunDssOnDatabase
    {
        void Execute(IJobCancellationToken token);
    }

    public class RunDssOnDatabase : IRunDssOnDatabase
    {
        private readonly IDataService dataService;
        public RunDssOnDatabase(IDataService dataService)
        {
            this.dataService = dataService
                ?? throw new ArgumentNullException(nameof(dataService));
        }

        public void Execute(IJobCancellationToken token)
        {
            token.ThrowIfCancellationRequested();
            Task.Run(() => RunAllDssOnDatabase(DateTime.Now)).Wait();
        }

        private async Task RunAllDssOnDatabase(DateTime now)
        {
            var listOfDss = await this.dataService.CropPestDsses.FindAllAsync();

            foreach (var dss in listOfDss)
            {
                Console.WriteLine(dss.DssId);
            }
        }
    }
}