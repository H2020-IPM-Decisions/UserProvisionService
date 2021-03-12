using System;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using H2020.IPMDecisions.UPR.BLL.Providers;
using H2020.IPMDecisions.UPR.Core.Entities;
using H2020.IPMDecisions.UPR.Data.Core;
using Hangfire;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace H2020.IPMDecisions.UPR.BLL.ScheduleTasks
{
    public interface IRunDssOnDatabase
    {
        void ExecuteOnTheFlyDss(IJobCancellationToken token);
    }

    public class RunDssOnDatabase : IRunDssOnDatabase
    {
        private readonly IDataService dataService;
        private readonly IMicroservicesInternalCommunicationHttpProvider internalCommunicationProvider;
        private readonly ILogger<RunDssOnDatabase> logger;
        public RunDssOnDatabase(
            IDataService dataService,
            IMicroservicesInternalCommunicationHttpProvider internalCommunicationProvider,
            ILogger<RunDssOnDatabase> logger)
        {
            this.dataService = dataService
                ?? throw new ArgumentNullException(nameof(dataService));
            this.internalCommunicationProvider = internalCommunicationProvider
                ?? throw new ArgumentNullException(nameof(internalCommunicationProvider));
            this.logger = logger
                ?? throw new ArgumentNullException(nameof(logger));
        }

        public void ExecuteOnTheFlyDss(IJobCancellationToken token)
        {
            try
            {
                token.ThrowIfCancellationRequested();
                Task.Run(() => RunAllDssOnDatabase(DateTime.Now)).Wait();
            }
            catch (Exception ex)
            {
                logger.LogError(string.Format("Error in BLL - Error executing On the Fly DSS. {0}", ex.Message));
            }
        }

        private async Task RunAllDssOnDatabase(DateTime now)
        {
            var listOfDss = await this.dataService.FieldCropPestDsses.FindAllAsync();
#if DEBUG
            listOfDss = listOfDss.Take(1);
#endif

            foreach (var dss in listOfDss)
            {
                var dssInformation = await
                    internalCommunicationProvider
                    .GetDssInformationFromDssMicroservice(dss.CropPestDss.DssId, dss.CropPestDss.DssModelId);
                if (dssInformation == null) continue;
                if (string.IsNullOrEmpty(dssInformation.EndPoint)) continue;
                if (dssInformation.Type.ToLower() != "onthefly") continue;


                //get weather data
                HttpClient httpClient = new HttpClient();

                var farm = dss.FieldCropPest.FieldCrop.Field.Farm;
                var dataSource = farm.FarmWeatherDataSources.FirstOrDefault().Url;
                var weatherUrl = string.Format("{0}?longitude={1}&latitude={2}",
                    dataSource, farm.Location.Coordinate.X.ToString(), farm.Location.Coordinate.Y.ToString());
                var responseWeather = await httpClient.GetAsync(weatherUrl);

                string responseWeatherAsText = "";
                if (responseWeather.IsSuccessStatusCode)
                {
                    responseWeatherAsText = await responseWeather.Content.ReadAsStringAsync();
                }

                JObject jObject = JObject.Parse(dss.DssParameters.ToString());
                jObject["weatherData"] = JObject.Parse(responseWeatherAsText.ToString());

                var content = new StringContent(
                     jObject.ToString(),
                     Encoding.UTF8, "application/json");
                var response = await httpClient.PostAsync(dssInformation.EndPoint, content);
                if (response.IsSuccessStatusCode)
                {
                    var responseAsText = await response.Content.ReadAsStringAsync();

                    var dssResult = new FieldDssResult()
                    {
                        CreationDate = DateTime.Now,
                        Result = responseAsText
                    };
                    this.dataService.FieldCropPestDsses.AddDssResult(dss, dssResult);
                }
            }
            await this.dataService.CompleteAsync();
        }
    }
}