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

    public partial class DssRunningJobs : IDssRunningJobs
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

                var inputSchemaAsJson = JsonSchemaToJson.ToJsonObject(dssInformation.Execution.InputSchema, logger);

                // Add user parameters
                if (!string.IsNullOrEmpty(dss.DssParameters))
                {
                    AddUserParametersToDss(dss.DssParameters, inputSchemaAsJson);
                }

                // Check defaults
                foreach (var property in inputSchemaAsJson.Properties())
                {
                    if (property.Name.ToLower() == "weatherdata")
                    {
                        continue;
                    }

                    var missingValueProperty = ProcessChildProperties(property);
                    if (!string.IsNullOrEmpty(missingValueProperty))
                    {
                        dssResult.ResultMessageType = (int)DssOutputMessageTypeEnum.Error;
                        dssResult.ResultMessage = string.Format("Property {0} do not have a value.", missingValueProperty);
                        dssResult.DssFullResult = JObject.Parse("{\"message\": \"Missing user DSS parameters input.\"}").ToString();
                        return dssResult;
                    }
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
                logger.LogError(string.Format("Error running DSS. Id: {0}, Parameters {1}. Error: {2}", dss.Id.ToString(), dss.DssParameters, ex.Message));
                dssResult.ResultMessageType = (int)DssOutputMessageTypeEnum.Error;
                dssResult.ResultMessage = ex.Message.ToString();
                dssResult.DssFullResult = JObject.Parse("{\"message\": \"Error running the DSS - " + ex.Message.ToString() + " \"}").ToString();
                return dssResult;
            }
        }

        private string ProcessChildProperties(JProperty property)
        {
            foreach (var childrenProperty in property.Children())
            {
                var hasMissingValue = "";
                if (childrenProperty.Type.ToString().ToLower() == "object")
                {
                    hasMissingValue = ProcessObjectChildProperty(childrenProperty);
                }
                else
                {
                    hasMissingValue = ProcessOtherTypeChildProperty(childrenProperty);
                }
                if (!string.IsNullOrEmpty(hasMissingValue))
                    return hasMissingValue;
            }
            return "";
        }

        private string ProcessOtherTypeChildProperty(JToken childrenProperty)
        {
            var value = ((JValue)childrenProperty).Value.ToString();
            if (string.IsNullOrEmpty(value))
                return childrenProperty.Path;
            return "";
        }

        private static string ProcessObjectChildProperty(JToken childrenProperty)
        {
            foreach (var property in childrenProperty.Children())
            {
                var y = ((JProperty)property);

                var value = ((JProperty)property).Value.ToString();
                if (string.IsNullOrEmpty(value))
                    return property.Path;
            }
            return "";
        }

        private static void AddUserParametersToDss(string userDssParameters, JObject inputSchemaAsJson)
        {
            JObject userInputJsonObject = JObject.Parse(userDssParameters.ToString());
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
    }
}