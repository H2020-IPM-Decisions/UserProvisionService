using System;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using H2020.IPMDecisions.UPR.BLL.Providers;
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
                logger.LogError(string.Format("Error in BLL - DeleteDataShareRequest. {0}", ex.Message));
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
                var dataSource = farm.FarmWeatherDataSources.FirstOrDefault().WeatherDataSource.Url;
                var weatherUrl = string.Format("{0}?longitude={1}&latitude={2}",
                    dataSource, farm.Location.Coordinate.X.ToString(), farm.Location.Coordinate.Y.ToString());
                var responseWeather = await httpClient.GetAsync(weatherUrl);

                string responseWeatherAsText = "";
                if (responseWeather.IsSuccessStatusCode)
                {
                    responseWeatherAsText = await responseWeather.Content.ReadAsStringAsync();
                }

                responseWeatherAsText = "{\r\n    \"timeStart\": \"2020-04-30T22:00:00Z\",\r\n    \"timeEnd\": \"2020-05-02T22:00:00Z\",\r\n    \"interval\": 86400,\r\n    \"weatherParameters\": [\r\n        1002\r\n    ],\r\n    \"locationWeatherData\": [\r\n        {\r\n            \"longitude\": 10.781989,\r\n            \"latitude\": 59.660468,\r\n            \"altitude\": 94.0,\r\n            \"data\": [\r\n                [\r\n                    5.7\r\n                ],\r\n                [\r\n                    8.2\r\n                ],\r\n                [\r\n                    8.5\r\n                ]\r\n            ],\r\n            \"length\": 3,\r\n            \"width\": 1\r\n        }\r\n    ]\r\n  }";

                JObject jObject = JObject.Parse(dss.DssParameters.ToString());
                jObject["weatherData"] = JObject.Parse(responseWeatherAsText.ToString());

                var content = new StringContent(
                     jObject.ToString(),
                     Encoding.UTF8, "application/json");
                var response = await httpClient.PostAsync(dssInformation.EndPoint, content);
                if (response.IsSuccessStatusCode)
                {
                    var responseAsText = await response.Content.ReadAsStringAsync();
                }
            }
        }
    }
}