using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using H2020.IPMDecisions.UPR.BLL.Helpers;
using H2020.IPMDecisions.UPR.BLL.Providers;
using H2020.IPMDecisions.UPR.Core.Entities;
using H2020.IPMDecisions.UPR.Core.Enums;
using H2020.IPMDecisions.UPR.Core.Models;
using H2020.IPMDecisions.UPR.Data.Core;
using Hangfire;
using Hangfire.Server;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Schema;

namespace H2020.IPMDecisions.UPR.BLL.ScheduleTasks
{
    public interface IDssRunningJobs
    {
        void ExecuteOnTheFlyDss(IJobCancellationToken token);
        void ExecuteDssWithErrors(IJobCancellationToken token);
        Task QueueOnTheFlyDss(IJobCancellationToken token, Guid dssId);
        Task QueueOnMemoryDss(IJobCancellationToken token, Guid id, string dssParameters, PerformContext context);
    }

    public partial class DssRunningJobs : IDssRunningJobs
    {
        private HttpClient httpClient;

        private readonly IDataService dataService;
        private readonly IMicroservicesInternalCommunicationHttpProvider internalCommunicationProvider;
        private readonly ILogger<DssRunningJobs> logger;
        private readonly IDataProtectionProvider dataProtectionProvider;
        private readonly IMapper mapper;
        private readonly IJsonStringLocalizer jsonStringLocalizer;
        private readonly IHangfireQueueJobs queueJobs;
        private readonly IMemoryCache memoryCache;
        private readonly IConfiguration config;
        private readonly EncryptionHelper _encryption;

        public DssRunningJobs(
            IDataService dataService,
            IMicroservicesInternalCommunicationHttpProvider internalCommunicationProvider,
            ILogger<DssRunningJobs> logger,
            IDataProtectionProvider dataProtectionProvider,
            IMapper mapper,
            IJsonStringLocalizer jsonStringLocalizer,
            IHangfireQueueJobs queueJobs,
            IMemoryCache memoryCache,
            IConfiguration config)
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
            this.jsonStringLocalizer = jsonStringLocalizer
                ?? throw new ArgumentNullException(nameof(jsonStringLocalizer));
            this.queueJobs = queueJobs
                ?? throw new ArgumentNullException(nameof(queueJobs));
            this.memoryCache = memoryCache
                ?? throw new ArgumentNullException(nameof(memoryCache));
            this.config = config
                ?? throw new ArgumentNullException(nameof(config));
            _encryption = new EncryptionHelper(dataProtectionProvider);
        }

        [Queue("onthefly_schedule")]
        public void ExecuteOnTheFlyDss(IJobCancellationToken token)
        {
            try
            {
                token.ThrowIfCancellationRequested();
                Task.Run(() => RunAllDssOnDatabase()).Wait();
            }
            catch (Exception ex)
            {
                logger.LogError(string.Format("Error in BLL - Error executing On the Fly DSS. {0}", ex.Message));
            }
        }

        [Queue("dsserror_schedule")]
        public void ExecuteDssWithErrors(IJobCancellationToken token)
        {
            try
            {
                token.ThrowIfCancellationRequested();
                Task.Run(() => RunAllDssOnDatabase(true)).Wait();
            }
            catch (Exception ex)
            {
                logger.LogError(string.Format("Error in BLL - Error executing On the Fly DSS. {0}", ex.Message));
            }
        }

        private async Task RunAllDssOnDatabase(bool addReScheduleFilter = false)
        {
            Expression<Func<FieldCropPestDss, bool>> expression = null;
            if (addReScheduleFilter)
            {
                expression = f =>
                    f.CropPestDss.DssExecutionType.ToLower().Equals("onthefly") &&
                    f.ReScheduleCount >= int.Parse(config["AppConfiguration:MaxReScheduleAttemptsDss"]);
            }
            else
            {
                expression = f =>
                    f.CropPestDss.DssExecutionType.ToLower().Equals("onthefly");
            }

            httpClient = new HttpClient();

            var listOfDss = await this
                .dataService
                .FieldCropPestDsses
                .FindAllAsync(expression);
            var count = listOfDss.Count();
            if (count == 0) return;
            logger.LogWarning("Total dss to run: {0}, Is RescheduleFilter?: {1}", count, addReScheduleFilter);
            TimeSpan lastEnqueuedTime = TimeSpan.FromSeconds(0);
            foreach (var dss in listOfDss)
            {
                // #if DEBUG
                // Call one by one to help debuggin
                // var dssResult = await RunOnTheFlyDss(dss);
                // if (dssResult == null) continue;
                // this.dataService.FieldCropPestDsses.AddDssResult(dss, dssResult);
                // #else
                // Add them to the queue every 10 seconds, to allow weather service to return data so doesn't get overload                
                dss.LastJobId = this.queueJobs.ScheduleDssOnTheFlyQueueTimeSpan(dss.Id, lastEnqueuedTime);
                lastEnqueuedTime = lastEnqueuedTime += TimeSpan.FromSeconds(10);
                // #endif
            }
            // Save last job ids and DSS results if debugging...
            await this.dataService.CompleteAsync();
            logger.LogWarning("Total DSSs to run: {0}, Last DSS will run in: {1}, Is RescheduleFilter?: {2}", count, lastEnqueuedTime, addReScheduleFilter);
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

        [Queue("onmemory_queue")]
        public async Task QueueOnMemoryDss(IJobCancellationToken token, Guid id, string dssParameters, PerformContext context)
        {
            try
            {
                token.ThrowIfCancellationRequested();
                await ExecuteOnMemoryDss(id, dssParameters, context);
            }
            catch (Exception ex)
            {
                logger.LogError(string.Format("Error in BLL - Error executing On the Fly DSS. {0}", ex.Message));
            }
        }

        [Queue("weather_queue")]
        public async Task QueueWeatherToAmalgamationService(IJobCancellationToken token, string weatherStringParametersUrl)
        {
            try
            {
                token.ThrowIfCancellationRequested();
                await
                    internalCommunicationProvider
                    .GetWeatherUsingAmalgamationService(weatherStringParametersUrl);
            }
            catch (Exception ex)
            {
                logger.LogError(string.Format("Error in BLL - Error executing Weather Queue. {0}", ex.Message));
            }
        }

        public async Task ExecuteDssOnQueue(Guid dssId)
        {
            try
            {
                httpClient = new HttpClient();
                var dss = await this.dataService.FieldCropPestDsses.FindByIdAsync(dssId);
                var dssResult = await RunOnTheFlyDss(dss);
                if (dssResult == null) return;
                this.dataService.FieldCropPestDsses.AddDssResult(dss, dssResult);
                await this.dataService.CompleteAsync();
            }
            catch (Exception ex)
            {
                logger.LogError(string.Format("Error in BLL - ExecuteDssOnQueue. {0}", ex.Message));
            }
            finally
            {
                httpClient.Dispose();
            }
        }

        private async Task ExecuteOnMemoryDss(Guid id, string dssParameters, PerformContext context)
        {
            try
            {
                httpClient = new HttpClient();
                var dss = await this.dataService.FieldCropPestDsses.FindByIdAsync(id);
                var dssResult = await RunOnTheFlyDss(dss, dssParameters);
                if (dssResult == null) return;
                string jobId = context.BackgroundJob.Id;
                var cacheKey = string.Format("InMemoryDssResult_{0}", jobId);
                memoryCache.Set(cacheKey, dssResult, MemoryCacheHelper.CreateMemoryCacheEntryOptionsMinutes(5));
            }
            catch (Exception ex)
            {
                logger.LogError(string.Format("Error in BLL - ExecuteDssOnQueue. {0}", ex.Message));
            }
            finally
            {
                httpClient.Dispose();
            }
        }

        #region DSS Common Stuff
        public async Task<FieldDssResult> RunOnTheFlyDss(FieldCropPestDss dss, string onTheFlyDssParameters = "")
        {
            var dssResult = new FieldDssResult()
            {
                CreationDate = DateTime.Now,
                IsValid = false,
                WarningStatus = 0
            };

            if (dss is null)
            {
                var errorMessage = this.jsonStringLocalizer["dss_process.system_ready_error"].ToString();
                CreateDssRunErrorResult(dssResult, errorMessage, DssOutputMessageTypeEnum.Error);
                return dssResult;
            }
            try
            {
                int maxReScheduleCount = int.Parse(config["AppConfiguration:MaxReScheduleAttemptsDss"]);
                DssModelInformation dssInformation = await GetDssInformationFromMicroservice(dss);
                if (dssInformation == null)
                {
                    CreateDssRunErrorResult(dssResult, this.jsonStringLocalizer["dss_process.dss_information"].ToString(), DssOutputMessageTypeEnum.Error);
                    return dssResult;
                };

                if (string.IsNullOrEmpty(dssInformation.Execution.EndPoint))
                {
                    CreateDssRunErrorResult(dssResult, this.jsonStringLocalizer["dss_process.dss_no_endpoint"].ToString(), DssOutputMessageTypeEnum.Error);
                    return dssResult;
                }
                if (dssInformation.Execution.Type.ToLower() != "onthefly") return null;

                var inputSchema = JsonSchemaToJson.StringToJsonSchema(dssInformation.Execution.InputSchema, logger);
                // Check required properties with Json Schema, remove not required
                DssDataHelper.RemoveNotRequiredInputSchemaProperties(inputSchema);

                JObject inputAsJsonObject = JsonSchemaToJson.ToJsonObject(inputSchema.ToString(), logger);
                // Add user parameters or parameters on the fly
                bool isOnTheFlyRun = !string.IsNullOrEmpty(onTheFlyDssParameters);
                if (isOnTheFlyRun)
                {
                    JObject dssParametersAsJsonObject = JObject.Parse(onTheFlyDssParameters.ToString());
                    DssDataHelper.AddUserDssParametersToDssInput(dssParametersAsJsonObject, inputAsJsonObject);
                }
                else if (!string.IsNullOrEmpty(dss.DssParameters))
                {
                    JObject dssParametersAsJsonObject = JObject.Parse(dss.DssParameters.ToString());
                    DssDataHelper.AddUserDssParametersToDssInput(dssParametersAsJsonObject, inputAsJsonObject);
                }

                IList<string> validationErrorMessages;
                bool isJsonObjectValid;
                bool reSchedule;
                ValidateJsonSchema(inputSchema, inputAsJsonObject, out validationErrorMessages, out isJsonObjectValid, out reSchedule);
                if (!isJsonObjectValid)
                {
                    string errorMessageToReturn = "";
                    if (reSchedule && dss.ReScheduleCount < maxReScheduleCount)
                    {
                        Random r = new Random();
                        var randomMinute = r.Next(0, 30);
                        errorMessageToReturn = this.jsonStringLocalizer["dss_process.json_validation_error", 61 + randomMinute].ToString();
                        var jobScheduleId = this.queueJobs.ScheduleDssOnTheFlyQueue(dss.Id, 61 + randomMinute);
                        dss.LastJobId = jobScheduleId;
                        dss.ReScheduleCount += 1;
                    }
                    else
                    {
                        errorMessageToReturn = string.Join(" ", validationErrorMessages);
                    }
                    CreateDssRunErrorResult(dssResult, errorMessageToReturn, DssOutputMessageTypeEnum.Error);
                    return dssResult;
                }

                if (dssInformation.Input.WeatherParameters != null)
                {
                    WeatherDataResult responseWeather = await PrepareWeatherData(dss, dssInformation, inputAsJsonObject, isOnTheFlyRun);
                    if (responseWeather.UpdateDssParameters)
                    {
                        dss.DssParameters = DssDataHelper.UpdateDssParametersToNewCalendarYear(dssInformation.Input.WeatherDataPeriodEnd, dss.DssParameters);
                        dss.DssParameters = DssDataHelper.UpdateDssParametersToNewCalendarYear(dssInformation.Input.WeatherDataPeriodStart, dss.DssParameters);
                    }

                    if (!responseWeather.Continue)
                    {
                        if (responseWeather.ResponseWeatherAsString.ToString().Contains("This is the first time this season that weather") && dss.ReScheduleCount < maxReScheduleCount)
                        {
                            var jobScheduleId = this.queueJobs.ScheduleDssOnTheFlyQueue(dss.Id, 121);
                            dss.LastJobId = jobScheduleId;
                            dss.ReScheduleCount += 1;
                        }
                        if (responseWeather.ReSchedule && dss.ReScheduleCount < maxReScheduleCount)
                        {
                            Random r = new Random();
                            var randomMinute = r.Next(0, 30);
                            var jobScheduleId = this.queueJobs.ScheduleDssOnTheFlyQueue(dss.Id, 30 + randomMinute);
                            dss.LastJobId = jobScheduleId;
                            dss.ReScheduleCount += 1;
                        }
                        var errorMessage = this.jsonStringLocalizer["dss_process.weather_data_error", responseWeather.ResponseWeatherAsString.ToString()].ToString();
                        CreateDssRunErrorResult(dssResult, errorMessage, responseWeather.ErrorType);
                        return dssResult;
                    }
                    inputAsJsonObject["weatherData"] = JObject.Parse(responseWeather.ResponseWeatherAsString.ToString());
                }
                dss.ReScheduleCount = 0;
                var content = new StringContent(
                     inputAsJsonObject.ToString(),
                     Encoding.UTF8, "application/json");

                var responseDss = await httpClient.PostAsync(dssInformation.Execution.EndPoint, content);
                await ProcessDssResult(dssResult, responseDss);
                return dssResult;
            }
            catch (Exception ex)
            {
                logger.LogError(string.Format("Error running DSS. Id: {0}, Parameters {1}. Error: {2}", dss.Id.ToString(), dss.DssParameters, ex.Message));
                var errorMessage = this.jsonStringLocalizer["dss_process.dss_error", ex.Message.ToString()].ToString();
                CreateDssRunErrorResult(dssResult, errorMessage, DssOutputMessageTypeEnum.Error);
                return dssResult;
            }
        }

        private void ValidateJsonSchema(JSchema inputSchema, JObject inputAsJsonObject, out IList<string> validationErrorMessages, out bool isJsonObjectValid, out bool reSchedule)
        {
            try
            {
                reSchedule = false;
                isJsonObjectValid = inputAsJsonObject.IsValid(inputSchema, out validationErrorMessages);
            }
            catch (Exception ex)
            {
                logger.LogError(string.Format("Error running DSS. Error: {0}", ex.Message));
                isJsonObjectValid = false;
                reSchedule = true;
                validationErrorMessages = null;
            }
        }

        private async Task ProcessDssResult(FieldDssResult dssResult, HttpResponseMessage responseDss)
        {
            try
            {
                var responseAsText = await responseDss.Content.ReadAsStringAsync();
                // check if response is JSON, if not, generic error
                if (!DataParseHelper.IsValidJson(responseAsText))
                {
                    logger.Log(LogLevel.Error, string.Format("Error running DSS: {0}. Response was: {1}",
                    responseDss.RequestMessage.RequestUri.ToString(),
                    responseAsText));

                    CreateDssRunErrorResult(dssResult, responseAsText, DssOutputMessageTypeEnum.Error);
                    return;
                };
                var dssOutput = JsonConvert.DeserializeObject<DssModelOutputInformation>(responseAsText);
                // ToDo. Check valid responses when DSS do not run properly
                if (!responseDss.IsSuccessStatusCode)
                {
                    logger.Log(LogLevel.Error, string.Format("Error running DSS: {0}. Response was: {1}",
                    responseDss.RequestMessage.RequestUri.ToString(),
                    responseAsText));

                    // DSS error follow schema output
                    if (!string.IsNullOrEmpty(dssOutput.Message))
                    {
                        if (dssOutput.MessageType == null)
                        {
                            dssResult.ResultMessageType = (int)DssOutputMessageTypeEnum.Error;
                        }
                        else
                        {
                            dssResult.ResultMessageType = dssOutput.MessageType;
                        }
                        dssResult.ResultMessage = dssOutput.Message;
                        dssResult.DssFullResult = responseAsText;
                        return;
                    }

                    // DSS do not follow DSS schema output...
                    var errorMessage = responseAsText.ToString();
                    CreateDssRunErrorResult(dssResult, errorMessage, DssOutputMessageTypeEnum.Error);
                    return;
                }

                if (!string.IsNullOrEmpty(dssOutput.Message))
                {
                    dssResult.ResultMessage = dssOutput.Message;
                    if (dssOutput.MessageType == null)
                    {
                        dssResult.ResultMessageType = (int)DssOutputMessageTypeEnum.Info;
                    }
                    else
                    {
                        dssResult.ResultMessageType = dssOutput.MessageType;
                    }
                }

                if (dssOutput.LocationResult != null)
                {
                    var warningStatuses = dssOutput.LocationResult.FirstOrDefault().WarningStatus;
                    if (warningStatuses != null)
                    {
                        //Take last 7 days of Data
                        var maxDaysOutput = 7;
                        dssResult.WarningStatus = warningStatuses.TakeLast(maxDaysOutput).Max();
                    }
                    if (dssResult.WarningStatus == null) dssResult.WarningStatus = 0;
                    dssResult.IsValid = true;
                }
                dssResult.DssFullResult = responseAsText;
            }
            catch (Exception ex)
            {
                logger.Log(LogLevel.Error, string.Format("Error running this DSS: {0}. Response was: {1}",
                    responseDss.RequestMessage.RequestUri.ToString(),
                    await responseDss.Content.ReadAsStringAsync()));
                dssResult.ResultMessageType = (int)DssOutputMessageTypeEnum.Error;
                CreateDssRunErrorResult(dssResult, ex.Message, DssOutputMessageTypeEnum.Error);
                return;
            }
        }
        #endregion

        #region helpers
        private async Task<DssModelInformation> GetDssInformationFromMicroservice(FieldCropPestDss dss)
        {
            return await internalCommunicationProvider
                .GetDssModelInformationFromDssMicroservice(dss.CropPestDss.DssId, dss.CropPestDss.DssModelId);
        }

        private void CreateDssRunErrorResult(FieldDssResult dssResult, string errorMessage, DssOutputMessageTypeEnum errorType, bool addDssFullResult = true)
        {
            dssResult.ResultMessageType = (int)errorType;
            dssResult.ResultMessage = errorMessage.ToString();
            if (!addDssFullResult) return;
            if (DataParseHelper.IsValidJson(("{\"message\": \"" + errorMessage + "\"}")))
            {
                dssResult.DssFullResult = JObject.Parse("{\"message\": \"" + errorMessage + "\"}").ToString();
            }
            else
            {
                dssResult.DssFullResult = JObject.Parse("{\"message\": \"" + this.jsonStringLocalizer["dss_process.dss_format_error"].ToString() + "\"}").ToString();
            }
        }
        #endregion
    }
}