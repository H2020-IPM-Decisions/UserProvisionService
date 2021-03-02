using System;
using System.Threading.Tasks;
using H2020.IPMDecisions.UPR.BLL.Providers;
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
        private readonly IMicroservicesInternalCommunicationHttpProvider internalCommunicationProvider;
        public RunDssOnDatabase(
            IDataService dataService,
            IMicroservicesInternalCommunicationHttpProvider internalCommunicationProvider)
        {
            this.dataService = dataService
                ?? throw new ArgumentNullException(nameof(dataService));
            this.internalCommunicationProvider = internalCommunicationProvider
                ?? throw new ArgumentNullException(nameof(internalCommunicationProvider));
        }

        public void Execute(IJobCancellationToken token)
        {
            token.ThrowIfCancellationRequested();
            Task.Run(() => RunAllDssOnDatabase(DateTime.Now)).Wait();
        }

        private async Task RunAllDssOnDatabase(DateTime now)
        {
            var listOfDss = await this.dataService.FieldCropPestDsses.FindAllAsync();

            foreach (var dss in listOfDss)
            {
                var executionUrl = await internalCommunicationProvider.GetDssInformationFromDssMicroservice(dss.CropPestDss.DssId, dss.CropPestDss.DssModelId);
                if (string.IsNullOrEmpty(executionUrl)) continue;

                Console.WriteLine("Doing something else!");
            }
        }
    }
}