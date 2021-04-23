using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using H2020.IPMDecisions.UPR.BLL.Helpers;
using H2020.IPMDecisions.UPR.BLL.Providers;
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
        private readonly IMapper mapper;
        private EncryptionHelper _encryption;
        public DssRunningJobs(
            IDataService dataService,
            IMicroservicesInternalCommunicationHttpProvider internalCommunicationProvider,
            ILogger<DssRunningJobs> logger,
            IDataProtectionProvider dataProtectionProvider,
            IMapper mapper)
        {
            this.dataService = dataService
                ?? throw new ArgumentNullException(nameof(dataService));
            this.internalCommunicationProvider = internalCommunicationProvider
                ?? throw new ArgumentNullException(nameof(internalCommunicationProvider));
            this.logger = logger
                ?? throw new ArgumentNullException(nameof(logger));
            this.dataProtectionProvider = dataProtectionProvider
             ?? throw new ArgumentNullException(nameof(dataProtectionProvider));
            this.mapper = mapper
                ?? throw new ArgumentNullException(nameof(mapper));
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
            var count = this.dataService.FieldCropPestDsses.GetCount(f => f.CropPestDss.DssExecutionType.ToLower().Equals("onthefly"));
            if (count == 0) return;
            var totalRecordsOnBatch = 50;
            var totalBatches = System.Math.Ceiling((decimal)count / totalRecordsOnBatch);

            HttpClient httpClient = new HttpClient();
            for (int batchNumber = 1; batchNumber <= totalBatches; batchNumber++)
            {
                var listOfDss = await this
                    .dataService
                    .FieldCropPestDsses
                    .FindAllAsync(f => f.CropPestDss.DssExecutionType.ToLower().Equals("onthefly"), batchNumber, totalRecordsOnBatch);

                foreach (var dss in listOfDss)
                {
                    var dssResult = await RunOnTheFlyDss(httpClient, dss);
                    if (dssResult == null) continue;
                    this.dataService.FieldCropPestDsses.AddDssResult(dss, dssResult);
                }
                await this.dataService.CompleteAsync();
            }
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
                DssInformation dssInformation = await GetDssInformationFromMicroservice(dss);
                if (dssInformation == null)
                {
                    dssResult.Result = JObject.Parse("{\"message\": \"Error getting DSS information from  microservice.\"}").ToString();
                    return dssResult;
                };

                if (string.IsNullOrEmpty(dssInformation.Execution.EndPoint))
                {
                    dssResult.Result = JObject.Parse("{\"message\": \"End point not available to run DSS.\"}").ToString();
                    return dssResult;
                }
                if (dssInformation.Execution.Type.ToLower() != "onthefly") return null;

                var inputSchemaAsJson = JsonSchemaToJson.ToJsonObject(dssInformation.Execution.InputSchema);
                JObject userInputJsonObject = JObject.Parse(dss.DssParameters.ToString());
                if (userInputJsonObject == null)
                {
                    dssResult.Result = JObject.Parse("{\"message\": \"Missing user DSS parameters input.\"}").ToString();
                    return dssResult;
                }
                foreach (var property in userInputJsonObject.Properties())
                {
                    var token = inputSchemaAsJson.SelectToken(property.Name);
                    if (token != null)
                    {
                        token.Replace(userInputJsonObject.SelectToken(property.Name));
                        continue;
                    }
                    inputSchemaAsJson.Add(property.Name, userInputJsonObject.SelectToken(property.Name));
                }

                if (dssInformation.Input.WeatherParameters != null)
                {
                    var listOfPreferredWeatherDataSources = new List<WeatherSchemaForHttp>();
                    var farm = dss.FieldCropPest.FieldCrop.Field.Farm;
                    if (farm.WeatherForecast != null)
                    {
                        var weatherToCall = this.mapper.Map<WeatherSchemaForHttp>(farm.WeatherForecast);
                        listOfPreferredWeatherDataSources.Add(weatherToCall);
                    }
                    if (farm.WeatherHistorical != null)
                    {
                        var weatherToCall = this.mapper.Map<WeatherSchemaForHttp>(farm.WeatherHistorical);
                        if (dssInformation.Input.WeatherDataPeriodStart != null)
                        {
                            var weatherStartDateJsonLocation = dssInformation.Input.WeatherDataPeriodStart.Value.ToString();
                            weatherToCall.WeatherTimeStart = DateTime.Parse(inputSchemaAsJson.SelectTokens(weatherStartDateJsonLocation).FirstOrDefault().ToString());
                        }
                        if (dssInformation.Input.WeatherDataPeriodEnd != null)
                        {
                            var weatherEndDateJsonLocation = dssInformation.Input.WeatherDataPeriodEnd.Value.ToString();
                            weatherToCall.WeatherTimeEnd = DateTime.Parse(inputSchemaAsJson.SelectTokens(weatherEndDateJsonLocation).FirstOrDefault().ToString());
                        }
                        listOfPreferredWeatherDataSources.Add(weatherToCall);
                    }

                    var responseWeather = await GetWeatherData(httpClient, farm.Location.X.ToString(), farm.Location.Y.ToString(), listOfPreferredWeatherDataSources, dssInformation.Input);
                    if (!responseWeather.Continue)
                    {
                        dssResult.Result = responseWeather.ResponseWeather;
                        return dssResult;
                    }
                    inputSchemaAsJson["weatherData"] = JObject.Parse(responseWeather.ResponseWeather.ToString());
                }

                var content = new StringContent(
                     inputSchemaAsJson.ToString(),
                     Encoding.UTF8, "application/json");
                var responseDss = await httpClient.PostAsync(dssInformation.Execution.EndPoint, content);

                if (!responseDss.IsSuccessStatusCode)
                {
                    dssResult.Result = JObject.Parse("{\"message\": \"Error running the DSS on endPoint - " + responseDss.ReasonPhrase.ToString() + " \"}").ToString();
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

        private async Task<DssInformation> GetDssInformationFromMicroservice(FieldCropPestDss dss)
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
            List<WeatherSchemaForHttp> listWeatherDataSource,
            DssSchemaInput dssWeatherInput)
        {
            var result = new GetWeatherDataResult();
            if (listWeatherDataSource.Count < 1)
            {
                result.ResponseWeather = JObject.Parse("{\"message\": \"No Weather Data Sources or Weather Stations associated to the farm\"}").ToString();
                return result;
            }

            //ToDo - Ask what to do when multiple weather data sources associated to a farm. At the moment use only one
            var weatherDataSource = listWeatherDataSource.FirstOrDefault();

            foreach (var parameter in dssWeatherInput.WeatherParameters)
            {
                weatherDataSource.WeatherDssParameters = parameter.ParameterCode.ToString() + ", " + weatherDataSource.WeatherDssParameters;
            }
            weatherDataSource.Interval = dssWeatherInput.WeatherParameters.FirstOrDefault().Interval;
            var responseWeather = await MakeWeatherDataCall(httpClient, farmLocationX, farmLocationY, weatherDataSource);

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
                result.ResponseWeather = JObject.Parse("{\"message\": \"Weather data received failed the Weather validation schema. This might be because the weather data source selected do not accept weather parameters required by the DSS: " + weatherDataSource.WeatherDssParameters.ToString() + "\"}").ToString();
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
            WeatherSchemaForHttp weatherDataSource)
        {
            var weatherEndPoint = weatherDataSource.Url;

            var weatherUrl = "";
            if (weatherDataSource.IsForecast)
            {
                weatherUrl = string.Format("{0}?longitude={1}&latitude={2}",
                    weatherEndPoint, farmLocationX, farmLocationY);
            }
            else
            {
                weatherUrl = string.Format("{0}?interval={1}&timeStart={2}&timeEnd={3}&ignoreErrors=true",
                    weatherEndPoint,
                    weatherDataSource.Interval.ToString(),
                    weatherDataSource.WeatherTimeStart.ToString("yyyy-MM-dd"),
                    weatherDataSource.WeatherTimeEnd.ToString("yyyy-MM-dd"));
            }
            if (!string.IsNullOrEmpty(weatherDataSource.WeatherDssParameters))
            {
                weatherUrl = string.Format("{0}&parameters={1}",
                    weatherUrl, weatherDataSource.WeatherDssParameters);
            }
            // if (!string.IsNullOrEmpty(weatherDataSource.StationId))
            // {
            //     weatherUrl = string.Format("{0}&weatherStationId={1}",
            //         weatherUrl, weatherDataSource.StationId.ToString());
            // }
            return await httpClient.GetAsync(weatherUrl);
        }
        #endregion
    }
}