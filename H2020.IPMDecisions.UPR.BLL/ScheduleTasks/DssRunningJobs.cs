using System;
using System.Collections.Generic;
using System.Linq;
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
using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Schema;

namespace H2020.IPMDecisions.UPR.BLL.ScheduleTasks
{
    public interface IDssRunningJobs
    {
        void ExecuteOnTheFlyDss(IJobCancellationToken token);
        Task QueueOnTheFlyDss(IJobCancellationToken token, Guid dssId);
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
        private readonly EncryptionHelper _encryption;

        public DssRunningJobs(
            IDataService dataService,
            IMicroservicesInternalCommunicationHttpProvider internalCommunicationProvider,
            ILogger<DssRunningJobs> logger,
            IDataProtectionProvider dataProtectionProvider,
            IMapper mapper,
            IJsonStringLocalizer jsonStringLocalizer,
            IHangfireQueueJobs queueJobs)
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

            httpClient = new HttpClient();

            for (int batchNumber = 1; batchNumber <= totalBatches; batchNumber++)
            {
                var listOfDss = await this
                    .dataService
                    .FieldCropPestDsses
                    .FindAllAsync(f => f.CropPestDss.DssExecutionType.ToLower().Equals("onthefly"), batchNumber, totalRecordsOnBatch);

                foreach (var dss in listOfDss)
                {
#if DEBUG
                    // Call one by one to help debuggin
                    var dssResult = await RunOnTheFlyDss(dss);
                    if (dssResult == null) continue;
                    this.dataService.FieldCropPestDsses.AddDssResult(dss, dssResult);
#else
                    // Schedule them to run every 3 seconds, to allow weather service to return data so doesn't get overload               
                    dss.LastJobId = this.queueJobs.ScheduleDssOnTheFlyQueue(dss.Id, 0.05);
#endif
                }
                // Save last job ids and DSS results if debugging...
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

        #region DSS Common Stuff
        public async Task<FieldDssResult> RunOnTheFlyDss(FieldCropPestDss dss)
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

                var inputAsJsonObject = JsonSchemaToJson.ToJsonObject(inputSchema.ToString(), logger);
                // Add user parameters
                if (!string.IsNullOrEmpty(dss.DssParameters))
                {
                    JObject dssParametersAsJsonObject = JObject.Parse(dss.DssParameters.ToString());
                    DssDataHelper.AddUserDssParametersToDssInput(dssParametersAsJsonObject, inputAsJsonObject);
                }

                IList<string> validationErrormessages;
                bool isJsonObjectvalid = inputAsJsonObject.IsValid(inputSchema, out validationErrormessages);
                if (!isJsonObjectvalid)
                {
                    string errorMessageToReturn = string.Join(" ", validationErrormessages);
                    CreateDssRunErrorResult(dssResult, errorMessageToReturn, DssOutputMessageTypeEnum.Info);
                    return dssResult;
                }

                if (dssInformation.Input.WeatherParameters != null)
                {
                    WeatherDataResult responseWeather = await PrepareWeatherData(dss, dssInformation, inputAsJsonObject);
                    if (!responseWeather.Continue)
                    {
                        if (responseWeather.ResponseWeather.ToString().Contains("This is the first time this season that weather"))
                        {
                            var jobscheduleId = this.queueJobs.ScheduleDssOnTheFlyQueue(dss.Id, 121);
                            dss.LastJobId = jobscheduleId;
                        }
                        var errorMessage = this.jsonStringLocalizer["dss_process.weather_data_error", responseWeather.ResponseWeather.ToString()].ToString();
                        CreateDssRunErrorResult(dssResult, errorMessage, DssOutputMessageTypeEnum.Error);
                        return dssResult;
                    }
                    inputAsJsonObject["weatherData"] = JObject.Parse(responseWeather.ResponseWeather.ToString());
                }

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

        private async Task ProcessDssResult(FieldDssResult dssResult, HttpResponseMessage responseDss)
        {
            try
            {
                var responseAsText = await responseDss.Content.ReadAsStringAsync();
                var dssOutput = JsonConvert.DeserializeObject<DssModelOutputInformation>(responseAsText);
                // ToDo. Check valid responses when DSS do not run properly
                if (!responseDss.IsSuccessStatusCode)
                {
                    logger.Log(LogLevel.Error, string.Format("Error doing running this DSS: {0} with this payload: {1}. Response was: {2}",
                    responseDss.RequestMessage.RequestUri.ToString(),
                    await responseDss.RequestMessage.Content.ReadAsStringAsync(),
                    responseAsText));

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
                        dssResult.IsValid = false;
                        return;
                    }

                    var errorMessage = responseAsText.ToString();
                    CreateDssRunErrorResult(dssResult, errorMessage, DssOutputMessageTypeEnum.Error);
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
                    if (dssResult.WarningStatus == null) dssResult.WarningStatus = 0;
                }
                dssResult.DssFullResult = responseAsText;
                dssResult.IsValid = true;
            }
            catch (Exception ex)
            {
                dssResult.ResultMessageType = (int)DssOutputMessageTypeEnum.Error;
                dssResult.IsValid = false;
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

        private void CreateDssRunErrorResult(FieldDssResult dssResult, string errorMessage, DssOutputMessageTypeEnum errorType)
        {
            dssResult.ResultMessageType = (int)errorType;
            dssResult.ResultMessage = errorMessage.ToString();
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