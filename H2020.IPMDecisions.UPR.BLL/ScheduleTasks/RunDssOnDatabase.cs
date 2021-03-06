using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using H2020.IPMDecisions.UPR.BLL.Helpers;
using H2020.IPMDecisions.UPR.BLL.Providers;
using H2020.IPMDecisions.UPR.Core.Dtos;
using H2020.IPMDecisions.UPR.Core.Entities;
using H2020.IPMDecisions.UPR.Data.Core;
using Hangfire;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.Logging;
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
        private readonly IDataProtectionProvider dataProtectionProvider;
        private EncryptionHelper _encryption;
        public RunDssOnDatabase(
            IDataService dataService,
            IMicroservicesInternalCommunicationHttpProvider internalCommunicationProvider,
            ILogger<RunDssOnDatabase> logger,
            IDataProtectionProvider dataProtectionProvider)
        {
            this.dataService = dataService
                ?? throw new ArgumentNullException(nameof(dataService));
            this.internalCommunicationProvider = internalCommunicationProvider
                ?? throw new ArgumentNullException(nameof(internalCommunicationProvider));
            this.logger = logger
                ?? throw new ArgumentNullException(nameof(logger));
            this.dataProtectionProvider = dataProtectionProvider
             ?? throw new ArgumentNullException(nameof(dataProtectionProvider));

            _encryption = new EncryptionHelper(dataProtectionProvider);
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
            listOfDss = listOfDss.Take(10);
#endif
            HttpClient httpClient = new HttpClient();


            foreach (var dss in listOfDss)
            {
                var dssInformation = await
                    internalCommunicationProvider
                    .GetDssInformationFromDssMicroservice(dss.CropPestDss.DssId, dss.CropPestDss.DssModelId);
                if (dssInformation == null) continue;
                if (string.IsNullOrEmpty(dssInformation.EndPoint)) continue;
                if (dssInformation.Type.ToLower() != "onthefly") continue;


                //get weather data

                var farm = dss.FieldCropPest.FieldCrop.Field.Farm;

                var responseWeather = await GetWeatherData(httpClient, farm);
                var dssResult = new FieldDssResult() { CreationDate = DateTime.Now };
                if (!responseWeather.IsSuccessStatusCode)
                {
                    dssResult.Result = JObject.Parse("{\"message\": \"Error getting the weather data - " + responseWeather.ReasonPhrase.ToString() + " \"}").ToString();
                    this.dataService.FieldCropPestDsses.AddDssResult(dss, dssResult);
                    continue;
                }

                var responseWeatherAsText = await responseWeather.Content.ReadAsStringAsync();
                if (!DataParseHelper.IsValidJson(responseWeatherAsText))
                {
                    dssResult.Result = JObject.Parse("{\"message\": \"Weather data received in an incorrect format\"}").ToString();
                    this.dataService.FieldCropPestDsses.AddDssResult(dss, dssResult);
                    continue;
                };

                JObject jObject = JObject.Parse(dss.DssParameters.ToString());
                jObject["weatherData"] = JObject.Parse(responseWeatherAsText.ToString());

                var content = new StringContent(
                     jObject.ToString(),
                     Encoding.UTF8, "application/json");
                var responseDss = await httpClient.PostAsync(dssInformation.EndPoint, content);
                if (!responseDss.IsSuccessStatusCode)
                {
                    dssResult.Result = JObject.Parse("{\"message\": \"Error running the DSS - " + responseDss.ReasonPhrase.ToString() + " \"}").ToString();
                    this.dataService.FieldCropPestDsses.AddDssResult(dss, dssResult);
                    continue;
                }
                var responseAsText = await responseDss.Content.ReadAsStringAsync();
                dssResult.Result = responseAsText;
                this.dataService.FieldCropPestDsses.AddDssResult(dss, dssResult);
            }
            await this.dataService.CompleteAsync();
        }

        public async Task<HttpResponseMessage> GetWeatherData(HttpClient httpClient, Farm farm)
        {
            var httpResponse = new HttpResponseMessage();
            var dataSource = farm.FarmWeatherDataSources.FirstOrDefault();
            var weatherEndPoint = dataSource.Url;

            if (dataSource.AuthenticationRequired)
            {
                var contentData = new Dictionary<string, string>();
                contentData.Add("weatherStationId", dataSource.StationId.ToString());
                contentData.Add("interval", dataSource.Interval.ToString());
                contentData.Add("ignoreErrors", "true");
                contentData.Add("parameters", dataSource.Parameters.ToString());
                contentData.Add("timeStart", dataSource.TimeStart.ToString("yyyy-MM-dd"));
                contentData.Add("timeEnd", dataSource.TimeEnd.ToString("yyyy-MM-dd"));
                var credentialsAsObject = JsonSerializer.Deserialize<WeatherCredentials>(dataSource.Credentials);
                credentialsAsObject.Password = _encryption.Decrypt(credentialsAsObject.Password);
                var credentialAsString = JsonSerializer.Serialize(credentialsAsObject, new JsonSerializerOptions()
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                });
                contentData.Add("credentials", credentialAsString);

                var content = new FormUrlEncodedContent(contentData);
                content.Headers.Clear();
                content.Headers.Add("Content-Type", "application/x-www-form-urlencoded");
                return await httpClient.PostAsync(weatherEndPoint, content);
            }

            var weatherUrl = "";
            if (dataSource.IsForecast)
            {
                weatherUrl = string.Format("{0}?longitude={1}&latitude={2}",
                    weatherEndPoint, farm.Location.Coordinate.X.ToString(), farm.Location.Coordinate.Y.ToString());
            }
            else
            {
                weatherUrl = string.Format("{0}?weatherStationId={1}&interval={2}&timeStart={3}&timeEnd={4}&parameters={5}&ignoreErrors=true",
                    weatherEndPoint,
                    dataSource.StationId.ToString(),
                    dataSource.Interval.ToString(),
                    dataSource.TimeStart.ToString(),
                    dataSource.TimeEnd.ToString(),
                    dataSource.Parameters.ToString());
            }
            return await httpClient.GetAsync(weatherUrl);
        }
    }
}