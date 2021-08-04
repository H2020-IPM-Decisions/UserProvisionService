using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using H2020.IPMDecisions.UPR.BLL.Helpers;
using H2020.IPMDecisions.UPR.BLL.Providers;
using H2020.IPMDecisions.UPR.Core.Entities;
using H2020.IPMDecisions.UPR.Core.Enums;
using H2020.IPMDecisions.UPR.Core.Models;
using H2020.IPMDecisions.UPR.Data.Core;
using Hangfire;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
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
            var dssResult = new FieldDssResult() { CreationDate = DateTime.Now, IsValid = false };
            try
            {
                DssModelInformation dssInformation = await GetDssInformationFromMicroservice(dss);
                if (dssInformation == null)
                {
                    dssResult.DssFullResult = JObject.Parse("{\"message\": \"Error getting DSS information from  microservice.\"}").ToString();
                    dssResult.ResultMessageType = (int)DssOutputMessageTypeEnum.Error;
                    dssResult.ResultMessage = "Error getting DSS information from  microservice.";
                    return dssResult;
                };

                if (string.IsNullOrEmpty(dssInformation.Execution.EndPoint))
                {
                    dssResult.ResultMessageType = (int)DssOutputMessageTypeEnum.Error;
                    dssResult.ResultMessage = "End point not available to run DSS.";
                    dssResult.DssFullResult = JObject.Parse("{\"message\": \"End point not available to run DSS.\"}").ToString();
                    return dssResult;
                }
                if (dssInformation.Execution.Type.ToLower() != "onthefly") return null;

                var inputSchemaAsJson = JsonSchemaToJson.ToJsonObject(dssInformation.Execution.InputSchema);
                // ToDo Check if required inputs are provided by the user
                JObject userInputJsonObject = JObject.Parse(dss.DssParameters.ToString());
                if (userInputJsonObject == null)
                {
                    // ToDo All metadata should have default values... use defaults
                    dssResult.ResultMessageType = (int)DssOutputMessageTypeEnum.Error;
                    dssResult.ResultMessage = "Missing user DSS parameters input.";
                    dssResult.DssFullResult = JObject.Parse("{\"message\": \"Missing user DSS parameters input.\"}").ToString();
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
                    GetWeatherDataResult responseWeather = await PrepareWeatherData(dss, dssInformation, inputSchemaAsJson);

                    if (!responseWeather.Continue)
                    {
                        dssResult.ResultMessageType = (int)DssOutputMessageTypeEnum.Error;
                        dssResult.ResultMessage = responseWeather.ResponseWeather;
                        dssResult.DssFullResult = JObject.Parse("{\"message\": \"Error with Weather Data - " + responseWeather.ResponseWeather.ToString() + "\"}").ToString();
                        return dssResult;
                    }
                    inputSchemaAsJson["weatherData"] = JObject.Parse(responseWeather.ResponseWeather.ToString());
                }

                var content = new StringContent(
                     inputSchemaAsJson.ToString(),
                     Encoding.UTF8, "application/json");
                var responseDss = await httpClient.PostAsync(dssInformation.Execution.EndPoint, content);

                await ProcessDssResult(dssResult, responseDss);
                return dssResult;
            }
            catch (Exception ex)
            {
                logger.LogError(string.Format("Error running DSS. Id: {0}, Parameters {1}. Error: {2}", dss.Id.ToString(), dss.DssParameters.ToString(), ex.Message));
                dssResult.ResultMessageType = (int)DssOutputMessageTypeEnum.Error;
                dssResult.ResultMessage = ex.Message.ToString();
                dssResult.DssFullResult = JObject.Parse("{\"message\": \"Error running the DSS - " + ex.Message.ToString() + " \"}").ToString();
                return dssResult;
            }
        }

        private async Task<GetWeatherDataResult> PrepareWeatherData(FieldCropPestDss dss, DssModelInformation dssInformation, JObject dssInputSchemaAsJson)
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
                    weatherToCall.WeatherTimeStart = DateTime.Parse(dssInputSchemaAsJson.SelectTokens(weatherStartDateJsonLocation).FirstOrDefault().ToString());
                }
                if (dssInformation.Input.WeatherDataPeriodEnd != null)
                {
                    var weatherEndDateJsonLocation = dssInformation.Input.WeatherDataPeriodEnd.Value.ToString();
                    weatherToCall.WeatherTimeEnd = DateTime.Parse(dssInputSchemaAsJson.SelectTokens(weatherEndDateJsonLocation).FirstOrDefault().ToString());
                }
                listOfPreferredWeatherDataSources.Add(weatherToCall);
            }

            return await GetWeatherData(farm.Location.X.ToString(), farm.Location.Y.ToString(), listOfPreferredWeatherDataSources, dssInformation.Input);
        }

        private static async Task ProcessDssResult(FieldDssResult dssResult, HttpResponseMessage responseDss)
        {
            var responseAsText = await responseDss.Content.ReadAsStringAsync();
            var dssOutput = JsonConvert.DeserializeObject<DssModelOutputInformation>(responseAsText);

            // ToDo. Check valid responses when DSS do not run properly
            if (responseDss.StatusCode.Equals(HttpStatusCode.InternalServerError))
            {
                dssResult.ResultMessageType = (int)DssOutputMessageTypeEnum.Error;
                dssResult.ResultMessage = string.Format("DSS returned a Internal Server Error. {0}", responseAsText.ToString());
                dssResult.DssFullResult = JObject.Parse("{\"message\": \"Error running the DSS on endPoint\"}").ToString();
                return;
            }

            if (!string.IsNullOrEmpty(dssOutput.Message))
            {
                dssResult.ResultMessageType = dssOutput.MessageType;
                dssResult.ResultMessage = dssOutput.Message;
                dssResult.DssFullResult = responseAsText;
                dssResult.IsValid = false;
                return;
            }

            if (dssOutput.LocationResult != null)
            {
                var warningStatuses = dssOutput.LocationResult.FirstOrDefault().WarningStatus;
                //Take last 7 days of Data
                var maxDaysOutput = 7;
                dssResult.WarningStatus = warningStatuses.TakeLast(maxDaysOutput).Max();
            }
            dssResult.DssFullResult = responseAsText;
            dssResult.IsValid = true;
        }

        private async Task<DssModelInformation> GetDssInformationFromMicroservice(FieldCropPestDss dss)
        {
            return await internalCommunicationProvider
                .GetDssModelInformationFromDssMicroservice(dss.CropPestDss.DssId, dss.CropPestDss.DssModelId);
        }
        #endregion

        #region Weather Common Stuff
        public async Task<GetWeatherDataResult> GetWeatherData(
            string farmLocationX,
            string farmLocationY,
            List<WeatherSchemaForHttp> listWeatherDataSource,
            DssModelSchemaInput dssWeatherInput)
        {
            var result = new GetWeatherDataResult();
            if (listWeatherDataSource.Count < 1)
            {
                result.ResponseWeather = "No Weather Data Sources or Weather Stations associated to the farm";
                return result;
            }

            //ToDo - Ask what to do when multiple weather data sources associated to a farm. At the moment use only one
            var weatherDataSource = listWeatherDataSource.FirstOrDefault();

            List<string> parameterCodes = dssWeatherInput.WeatherParameters.Select(s => s.ParameterCode.ToString()).ToList();
            weatherDataSource.WeatherDssParameters = string.Join(",", parameterCodes);

            weatherDataSource.Interval = dssWeatherInput.WeatherParameters.FirstOrDefault().Interval;
            var responseWeather = await PrepareWeatherDataCall(farmLocationX, farmLocationY, weatherDataSource);

            result.Continue = false;
            if (!responseWeather.IsSuccessStatusCode)
            {
                result.ResponseWeather = string.Format("Error getting the weather data - {0}", responseWeather.ReasonPhrase.ToString());
                return result;
            }

            var responseWeatherAsText = await responseWeather.Content.ReadAsStringAsync();
            if (!DataParseHelper.IsValidJson(responseWeatherAsText))
            {
                result.ResponseWeather = "Weather data received in not in a JSON format.";
                return result;
            };

            if (!await ValidateWeatherDataSchema(responseWeatherAsText))
            {
                result.ResponseWeather = string.Format("Weather data received failed the Weather validation schema. This might be because the weather data source selected do not accept weather parameters required by the DSS: {0}", weatherDataSource.WeatherDssParameters.ToString());
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

        public async Task<HttpResponseMessage> PrepareWeatherDataCall(
            string farmLocationX,
            string farmLocationY,
            WeatherSchemaForHttp weatherDataSource)
        {
            var weatherStringParametersUrl = "";
            if (weatherDataSource.IsForecast)
            {
                weatherStringParametersUrl = string.Format("longitude={0}&latitude={1}",
                    farmLocationX, farmLocationY);
            }
            else
            {
                weatherStringParametersUrl = string.Format("interval={0}&timeStart={1}&timeEnd={2}&ignoreErrors=true",
                    weatherDataSource.Interval.ToString(),
                    weatherDataSource.WeatherTimeStart.ToString("yyyy-MM-dd"),
                    weatherDataSource.WeatherTimeEnd.ToString("yyyy-MM-dd"));
            }
            if (!string.IsNullOrEmpty(weatherDataSource.WeatherDssParameters))
            {
                weatherStringParametersUrl = string.Format("{0}&parameters={1}",
                    weatherStringParametersUrl, weatherDataSource.WeatherDssParameters);
            }
            // if (!string.IsNullOrEmpty(weatherDataSource.StationId))
            // {
            //     weatherUrl = string.Format("{0}&weatherStationId={1}",
            //         weatherUrl, weatherDataSource.StationId.ToString());
            // }
            return await
                    internalCommunicationProvider
                    .GetWeatherUsingAmalgamationService(weatherDataSource.Url, weatherStringParametersUrl);
        }
        #endregion
    }
}