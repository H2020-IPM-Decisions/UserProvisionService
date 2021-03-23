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
using H2020.IPMDecisions.UPR.Core.Models;
using H2020.IPMDecisions.UPR.Data.Core;
using Hangfire;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;

namespace H2020.IPMDecisions.UPR.BLL.ScheduleTasks
{
    public interface IDssRunningJobs
    {
        void ExecuteOnTheFlyDss(IJobCancellationToken token);
        Task QueueOnTheFlyDss(IJobCancellationToken token, Guid dssId);
    }

    public class DssRunningJobs : IDssRunningJobs
    {
        private readonly IDataService dataService;
        private readonly IMicroservicesInternalCommunicationHttpProvider internalCommunicationProvider;
        private readonly ILogger<DssRunningJobs> logger;
        private readonly IDataProtectionProvider dataProtectionProvider;
        private EncryptionHelper _encryption;
        public DssRunningJobs(
            IDataService dataService,
            IMicroservicesInternalCommunicationHttpProvider internalCommunicationProvider,
            ILogger<DssRunningJobs> logger,
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

        [Queue("onthefly_schedule")]
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
            // listOfDss = listOfDss.Take(20);
            // listOfDss = listOfDss.Where(s => s.FieldCropPestId == Guid.Parse("baaf6737-751b-4cad-8bde-216a2df330fd"));
#endif
            HttpClient httpClient = new HttpClient();
            foreach (var dss in listOfDss)
            {
                var dssResult = await RunOnTheFlyDss(httpClient, dss);
                if (dssResult == null) continue;
                this.dataService.FieldCropPestDsses.AddDssResult(dss, dssResult);
            }
            await this.dataService.CompleteAsync();
        }

        [Queue("onthefly_queue")]
        public async Task QueueOnTheFlyDss(IJobCancellationToken token, Guid id)
        {
            try
            {
                token.ThrowIfCancellationRequested();
                await ExecuteDssOnQueue(id);
            }
            catch (Exception ex)
            {
                logger.LogError(string.Format("Error in BLL - Error executing On the Fly DSS. {0}", ex.Message));
            }
        }

        public async Task ExecuteDssOnQueue(Guid dssId)
        {
            try
            {
                HttpClient httpClient = new HttpClient();
                var dss = await this.dataService.FieldCropPestDsses.FindByIdAsync(dssId);
                var dssResult = await RunOnTheFlyDss(httpClient, dss);
                if (dssResult == null) return;
                this.dataService.FieldCropPestDsses.AddDssResult(dss, dssResult);
                await this.dataService.CompleteAsync();
            }
            catch (Exception ex)
            {
                logger.LogError(string.Format("Error in BLL - ExecuteDssOnQueue. {0}", ex.Message));
            }
        }

        #region DSS Common Stuff
        public async Task<FieldDssResult> RunOnTheFlyDss(HttpClient httpClient, FieldCropPestDss dss)
        {
            var dssResult = new FieldDssResult() { CreationDate = DateTime.Now };
            try
            {
                DssExecutionInformation dssInformation = await GetDssInformationFromMicroservice(dss);

                if (dssInformation == null)
                {
                    dssResult.Result = JObject.Parse("{\"message\": \"Error getting DSS information from  microservice.\"}").ToString();
                    return dssResult;
                };

                if (string.IsNullOrEmpty(dssInformation.EndPoint))
                {
                    dssResult.Result = JObject.Parse("{\"message\": \"End point not available to run DSS.\"}").ToString();
                    return dssResult;
                }
                if (dssInformation.Type.ToLower() != "onthefly") return null;

                JObject jObject = JObject.Parse(dss.DssParameters.ToString());
                if (dssInformation.UsesWeatherData)
                {
                    var farm = dss.FieldCropPest.FieldCrop.Field.Farm;
                    var dataSource = farm.FarmWeatherDataSources.FirstOrDefault();

                    var responseWeather = await GetWeatherData(httpClient, farm.Location.X.ToString(), farm.Location.Y.ToString(), dataSource, dssInformation.WeatherParameters);
                    if (!responseWeather.Continue)
                    {
                        dssResult.Result = responseWeather.ResponseWeather;
                        return dssResult;
                    }
                    jObject["weatherData"] = JObject.Parse(responseWeather.ResponseWeather.ToString());
                }

                var content = new StringContent(
                     jObject.ToString(),
                     Encoding.UTF8, "application/json");
                var responseDss = await httpClient.PostAsync(dssInformation.EndPoint, content);

                if (!responseDss.IsSuccessStatusCode)
                {
                    dssResult.Result = JObject.Parse("{\"message\": \"Error running the DSS - " + responseDss.ReasonPhrase.ToString() + " \"}").ToString();
                    return dssResult;
                }
                var responseAsText = await responseDss.Content.ReadAsStringAsync();
                dssResult.Result = responseAsText;
                return dssResult;
            }
            catch (Exception ex)
            {
                logger.LogError(string.Format("Error running DSS. Id: {0}, Parameters {1}. Error: {2}", dss.Id.ToString(), dss.DssParameters.ToString(), ex.Message));
                dssResult.Result = JObject.Parse("{\"message\": \"Error running the DSS - " + ex.Message.ToString() + " \"}").ToString();
                return dssResult;
            }
        }

        private async Task<DssExecutionInformation> GetDssInformationFromMicroservice(FieldCropPestDss dss)
        {
            return await internalCommunicationProvider
                .GetDssInformationFromDssMicroservice(dss.CropPestDss.DssId, dss.CropPestDss.DssModelId);
        }
        #endregion

        #region Weather Common Stuff
        public async Task<GetWeatherDataResult> GetWeatherData(
            HttpClient httpClient,
            string farmLocationX,
            string farmLocationY,
            WeatherDataSource dataSource,
            string dssWeatherParameters)
        {
            var responseWeather = await MakeWeatherDataCall(httpClient, farmLocationX, farmLocationY, dataSource, dssWeatherParameters);
            var result = new GetWeatherDataResult();
            result.Continue = false;
            if (!responseWeather.IsSuccessStatusCode)
            {
                result.ResponseWeather = JObject.Parse("{\"message\": \"Error getting the weather data - " + responseWeather.ReasonPhrase.ToString() + " \"}").ToString();
                return result;
            }

            var responseWeatherAsText = await responseWeather.Content.ReadAsStringAsync();
            if (!DataParseHelper.IsValidJson(responseWeatherAsText))
            {
                result.ResponseWeather = JObject.Parse("{\"message\": \"Weather data received in not in a JSON format.\"}").ToString();
                return result;
            };

            if (!await ValidateWeatherDataSchema(responseWeatherAsText))
            {
                result.ResponseWeather = JObject.Parse("{\"message\": \"Weather data received failed the Weather validation schema. This might be because the weather data source selected do not accept weather parameters required by the DSS: " + dssWeatherParameters.ToString() + "\"}").ToString();
                return result;
            };
            result.Continue = true;
            result.ResponseWeather = responseWeatherAsText;
            return result;
        }

        public async Task<bool> ValidateWeatherDataSchema(string weatherDataSchema)
        {
            try
            {
                return await
                    internalCommunicationProvider
                    .ValidateWeatherdDataSchemaFromDssMicroservice(weatherDataSchema);
            }
            catch (Exception ex)
            {
                logger.LogError(string.Format("Error Validating Weather Data Schema. {0}", ex.Message));
                return false;
            }
        }

        public async Task<HttpResponseMessage> MakeWeatherDataCall(
            HttpClient httpClient,
            string farmLocationX,
            string farmLocationY,
            WeatherDataSource weatherDataSource,
            string dssWeatherParameters)
        {
            var weatherEndPoint = weatherDataSource.Url;

            if (weatherDataSource.AuthenticationRequired)
            {
                var contentData = new Dictionary<string, string>();
                contentData.Add("weatherStationId", weatherDataSource.StationId.ToString());
                contentData.Add("interval", weatherDataSource.Interval.ToString());
                contentData.Add("ignoreErrors", "true");
                contentData.Add("timeStart", weatherDataSource.TimeStart.ToString("yyyy-MM-dd"));
                contentData.Add("timeEnd", weatherDataSource.TimeEnd.ToString("yyyy-MM-dd"));
                var credentialsAsObject = JsonSerializer.Deserialize<WeatherCredentials>(weatherDataSource.Credentials);
                credentialsAsObject.Password = _encryption.Decrypt(credentialsAsObject.Password);
                var credentialAsString = JsonSerializer.Serialize(credentialsAsObject, new JsonSerializerOptions()
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                });
                contentData.Add("credentials", credentialAsString);

                if (!string.IsNullOrEmpty(dssWeatherParameters))
                {
                    contentData.Add("parameters", dssWeatherParameters);
                }

                var content = new FormUrlEncodedContent(contentData);
                content.Headers.Clear();
                content.Headers.Add("Content-Type", "application/x-www-form-urlencoded");
                return await httpClient.PostAsync(weatherEndPoint, content);
            }

            var weatherUrl = "";
            if (weatherDataSource.IsForecast)
            {
                weatherUrl = string.Format("{0}?longitude={1}&latitude={2}",
                    weatherEndPoint, farmLocationX, farmLocationY);
            }
            else
            {
                weatherUrl = string.Format("{0}?weatherStationId={1}&interval={2}&timeStart={3}&timeEnd={4}&ignoreErrors=true",
                    weatherEndPoint,
                    weatherDataSource.StationId.ToString(),
                    weatherDataSource.Interval.ToString(),
                    weatherDataSource.TimeStart.ToString("yyyy-MM-dd"),
                    weatherDataSource.TimeEnd.ToString("yyyy-MM-dd"));
            }
            if (!string.IsNullOrEmpty(dssWeatherParameters))
            {
                weatherUrl = string.Format("{0}&parameters={1}",
                    weatherUrl, dssWeatherParameters);
            }
            return await httpClient.GetAsync(weatherUrl);
        }
        #endregion
    }
}