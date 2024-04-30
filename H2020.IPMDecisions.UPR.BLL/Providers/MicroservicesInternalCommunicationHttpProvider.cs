using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net;
using System.Net.Http;
using System.Net.Mime;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using H2020.IPMDecisions.UPR.BLL.Helpers;
using H2020.IPMDecisions.UPR.Core.Models;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Schema;

namespace H2020.IPMDecisions.UPR.BLL.Providers
{
    public class MicroservicesInternalCommunicationHttpProvider : IMicroservicesInternalCommunicationHttpProvider, IDisposable
    {
        private readonly HttpClient httpClient;
        private readonly IConfiguration config;
        private readonly ILogger<MicroservicesInternalCommunicationHttpProvider> logger;
        private readonly IMemoryCache memoryCache;

        public MicroservicesInternalCommunicationHttpProvider(
            HttpClient httpClient,
            IConfiguration config,
            ILogger<MicroservicesInternalCommunicationHttpProvider> logger,
            IMemoryCache memoryCache)
        {
            this.httpClient = httpClient ?? throw new System.ArgumentNullException(nameof(httpClient));
            this.config = config ?? throw new System.ArgumentNullException(nameof(config));
            this.logger = logger ?? throw new System.ArgumentNullException(nameof(logger));
            this.memoryCache = memoryCache ?? throw new ArgumentNullException(nameof(memoryCache));
        }

        public void Dispose()
        {
            httpClient?.Dispose();
        }

        #region DSS Microservice calls
        public async Task<DssModelInformation> GetDssModelInformationFromDssMicroservice(string dssId, string modelId)
        {
            try
            {
                var language = Thread.CurrentThread.CurrentCulture.Name;
                var dssEndPoint = config["MicroserviceInternalCommunication:DssMicroservice"];
                var response = await httpClient.GetAsync(string.Format("{0}rest/model/{1}/{2}?language={3}", dssEndPoint, dssId, modelId, language));

                if (!response.IsSuccessStatusCode) return null;

                var responseAsText = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<DssModelInformation>(responseAsText);
            }
            catch (Exception ex)
            {
                logger.LogError(string.Format("Error in Internal Communication - GetDssInformationFromDssMicroservice. {0}", ex.Message));
                return null;
            }
        }

        public async Task<IEnumerable<DssInformation>> GetAllListOfDssFromDssMicroservice()
        {
            try
            {
                var language = Thread.CurrentThread.CurrentCulture.Name;
                var cacheKey = string.Format("listOfDss_{0}", language);
                if (!memoryCache.TryGetValue(cacheKey, out IEnumerable<DssInformation> listOfDss))
                {
                    var dssEndPoint = config["MicroserviceInternalCommunication:DssMicroservice"];
                    var response = await httpClient.GetAsync(string.Format("{0}rest/dss?language={1}", dssEndPoint, language));

                    if (!response.IsSuccessStatusCode)
                        return null;

                    var responseAsText = await response.Content.ReadAsStringAsync();
                    listOfDss = JsonConvert.DeserializeObject<IEnumerable<DssInformation>>(responseAsText);
                    memoryCache.Set(cacheKey, listOfDss, MemoryCacheHelper.CreateMemoryCacheEntryOptionsMinutes(30));
                }
                return listOfDss;
            }
            catch (Exception ex)
            {
                logger.LogError(string.Format("Error in Internal Communication - GetAllListOfDssFromDssMicroservice. {0}", ex.Message));
                return null;
            }
        }

        public async Task<JSchema> GetDssModelInputSchemaMicroservice(string dssId, string modelId)
        {
            try
            {
                var language = Thread.CurrentThread.CurrentCulture.Name;

                var dssEndPoint = config["MicroserviceInternalCommunication:DssMicroservice"];
                var response = await httpClient.GetAsync(string.Format("{0}rest/model/{1}/{2}/input_schema/ui_form?language={3}",
                    dssEndPoint, dssId, modelId, language));

                if (!response.IsSuccessStatusCode) return null;
                var responseAsString = await response.Content.ReadAsStringAsync();
                JSchemaUrlResolver resolver = new JSchemaUrlResolver();
                return JSchema.Parse(responseAsString, resolver);
            }
            catch (Exception ex)
            {
                logger.LogError(string.Format("Error in Internal Communication - GetDssModelInputSchemaMicroservice. {0}", ex.Message));
                return null;
            }
        }

        public async Task<List<string>> GetListOfEppoCodesFromDssMicroservice(string eppoCodeType, string executionType)
        {
            try
            {
                var language = Thread.CurrentThread.CurrentCulture.Name;
                var cacheKey = string.Format("eppoCodes_{0}_{1}_{2}", eppoCodeType.ToLower(), language, executionType);
                if (!memoryCache.TryGetValue(cacheKey, out List<string> listOfEppoCodes))
                {
                    var dssEndPoint = config["MicroserviceInternalCommunication:DssMicroservice"];
                    var filterString = "";
                    if (!eppoCodeType.ToLower().Equals("pest"))
                    {
                        filterString = "platform_validated/true";
                    }
                    if (!string.IsNullOrEmpty(executionType))
                    {
                        filterString = $"execution_type/{executionType.ToUpper()}";
                    }

                    var response = await httpClient.GetAsync(string.Format("{0}rest/{1}/{2}", dssEndPoint, eppoCodeType, filterString));

                    if (!response.IsSuccessStatusCode) return null;

                    var responseAsText = await response.Content.ReadAsStringAsync();
                    listOfEppoCodes = JsonConvert.DeserializeObject<List<string>>(responseAsText);

                    memoryCache.Set(cacheKey, listOfEppoCodes, MemoryCacheHelper.CreateMemoryCacheEntryOptionsMinutes(30));
                }
                return listOfEppoCodes;
            }
            catch (Exception ex)
            {
                logger.LogError(string.Format("Error in Internal Communication - GetListOfEppoCodesFromDssMicroservice. {0}", ex.Message));
                return null;
            }
        }

        public async Task<IEnumerable<DssInformation>> GetListOfDssByLocationFromDssMicroservice(GeoJsonFeatureCollection geoJson, string executionType = "")
        {
            try
            {
                var language = Thread.CurrentThread.CurrentCulture.Name;
                var cacheKey = string.Format("eppoCodes_{0}_{1}_{2}", geoJson.ToString().ToLower(), language, executionType);
                if (!memoryCache.TryGetValue(cacheKey, out string responseAsText))
                {
                    var content = new StringContent(
                        System.Text.Json.JsonSerializer.Serialize(geoJson),
                        Encoding.UTF8,
                        MediaTypeNames.Application.Json);
                    var dssEndPoint = config["MicroserviceInternalCommunication:DssMicroservice"];
                    var response = await httpClient.PostAsync(
                        string.Format("{0}rest/dss/location?language={1}&executionType={2}", dssEndPoint, language, executionType),
                        content);

                    if (!response.IsSuccessStatusCode)
                        return null;

                    responseAsText = await response.Content.ReadAsStringAsync();
                    memoryCache.Set(cacheKey, responseAsText, MemoryCacheHelper.CreateMemoryCacheEntryOptionsMinutes(30));
                }
                return JsonConvert.DeserializeObject<IEnumerable<DssInformation>>(responseAsText);
            }
            catch (Exception ex)
            {
                logger.LogError(string.Format("Error in Internal Communication - GetListOfDssByLocationFromDssMicroservice. {0}", ex.Message));
                return null;
            }
        }

        public async Task<DssInformation> GetDssInformationFromDssMicroservice(string dssId)
        {
            try
            {
                var language = Thread.CurrentThread.CurrentCulture.Name;
                var cacheKey = string.Format("dssInformation_{0}_{1}", dssId.ToLower(), language);
                if (!memoryCache.TryGetValue(cacheKey, out DssInformation dssInformation))
                {
                    var dssEndPoint = config["MicroserviceInternalCommunication:DssMicroservice"];
                    var response = await httpClient.GetAsync(string.Format("{0}rest/dss/{1}?language={2}", dssEndPoint, dssId, language));

                    if (!response.IsSuccessStatusCode) return null;

                    var responseAsText = await response.Content.ReadAsStringAsync();
                    dssInformation = JsonConvert.DeserializeObject<DssInformation>(responseAsText);

                    memoryCache.Set(cacheKey, dssInformation, MemoryCacheHelper.CreateMemoryCacheEntryOptionsMinutes(30));
                }
                return dssInformation;
            }
            catch (Exception ex)
            {
                logger.LogError(string.Format("Error in Internal Communication - GetDssInformationFromDssMicroservice. {0}", ex.Message));
                return null;
            }
        }
        #endregion

        #region Weather Microservice calls
        public async Task<string> GetUserIdFromIdpMicroservice(string userEmail)
        {
            try
            {
                var cacheKey = string.Format("user_{0}", userEmail.ToLower());
                if (!memoryCache.TryGetValue(cacheKey, out string userId))
                {
                    var jsonObject = new System.Json.JsonObject();
                    jsonObject.Add("email", userEmail);
                    var customContentType = config["MicroserviceInternalCommunication:ContentTypeHeader"];

                    var content = new StringContent(
                        jsonObject.ToString(),
                        Encoding.UTF8,
                        customContentType);

                    var idpEndPoint = config["MicroserviceInternalCommunication:IdentityProviderMicroservice"];
                    var userIdResponse = await httpClient.PostAsync(idpEndPoint + "internal/getuserid", content);

                    if (!userIdResponse.IsSuccessStatusCode)
                    {
                        return null;
                    }
                    userId = await userIdResponse.Content.ReadAsStringAsync();
                    memoryCache.Set(cacheKey, userId, MemoryCacheHelper.CreateMemoryCacheEntryOptionsDays(15));
                }
                return userId;
            }
            catch (Exception ex)
            {
                logger.LogError(string.Format("Error in Internal Communication - GetUserId. {0}", ex.Message));
                return null;
            }
        }

        public async Task<UserInformationInternalCall> GetUserInformationFromIdpMicroservice(string userEmail)
        {
            try
            {
                var cacheKey = string.Format("user_information_{0}", userEmail.ToLower());
                if (!memoryCache.TryGetValue(cacheKey, out UserInformationInternalCall userInformation))
                {
                    var jsonObject = new System.Json.JsonObject
                    {
                        { "email", userEmail }
                    };
                    var customContentType = config["MicroserviceInternalCommunication:ContentTypeHeader"];

                    var content = new StringContent(
                        jsonObject.ToString(),
                        Encoding.UTF8,
                        customContentType);

                    var idpEndPoint = config["MicroserviceInternalCommunication:IdentityProviderMicroservice"];
                    var userInformationResponse = await httpClient.PostAsync(idpEndPoint + "internal/get-user-information", content);

                    if (!userInformationResponse.IsSuccessStatusCode)
                    {
                        return null;
                    }
                    var responseAsText = await userInformationResponse.Content.ReadAsStringAsync();
                    userInformation = JsonConvert.DeserializeObject<UserInformationInternalCall>(responseAsText);
                    memoryCache.Set(cacheKey, userInformation, MemoryCacheHelper.CreateMemoryCacheEntryOptionsDays(15));
                }
                return userInformation;
            }
            catch (Exception ex)
            {
                logger.LogError(string.Format("Error in Internal Communication - GetUserId. {0}", ex.Message));
                return null;
            }
        }

        public async Task<bool> SendDataRequestEmail(string requesterEmail, string toEmail)
        {
            try
            {
                var jsonObject = new System.Json.JsonObject();
                jsonObject.Add("dataRequesterName", requesterEmail);
                jsonObject.Add("toAddress", toEmail);
                jsonObject.Add("language", Thread.CurrentThread.CurrentCulture.Name);
                var customContentType = config["MicroserviceInternalCommunication:ContentTypeHeader"];

                var content = new StringContent(
                    jsonObject.ToString(),
                    Encoding.UTF8,
                    customContentType);

                var emlEndPoint = config["MicroserviceInternalCommunication:EmailMicroservice"];
                var emailResponse = await httpClient.PostAsync(emlEndPoint + "internal/SendDataRequest", content);

                if (!emailResponse.IsSuccessStatusCode)
                {
                    var responseContent = await emailResponse.Content.ReadAsStringAsync();
                    logger.LogWarning(string.Format("Error in Sending Request Data Email. Reason: {0}. Response Content: {1}",
                        emailResponse.ReasonPhrase, responseContent));
                    return false;
                }
                return true;
            }
            catch (Exception ex)
            {
                logger.LogError(string.Format("Error in Internal Communication - SendDataRequestEmail. {0}", ex.Message));
                return false;
            }
        }

        public async Task<bool> ValidateWeatherDataSchemaFromDssMicroservice(string weatherDataSchema)
        {
            try
            {
                var content = new StringContent(
                                    weatherDataSchema,
                                    Encoding.UTF8,
                                    MediaTypeNames.Application.Json);

                var wxEndPoint = config["MicroserviceInternalCommunication:WeatherMicroservice"];
                var validationResponse = await httpClient.PostAsync(wxEndPoint + "rest/schema/weatherdata/validate", content);

                if (validationResponse.IsSuccessStatusCode)
                {
                    var response = await validationResponse.Content.ReadAsStringAsync();
                    JObject jObject = JObject.Parse(response);
                    return jObject["isValid"].Value<bool>();
                }
                return false;
            }
            catch (Exception ex)
            {
                logger.LogError(string.Format("Error in Internal Communication - ValidateWeatherdDataSchemaFromDssMicroservice. {0}", ex.Message));
                return false;
            }
        }

        public async Task<HttpResponseMessage> GetWeatherUsingAmalgamationPrivateService(string endPointQueryString, PrivateWeatherBodyRequest ownWeatherDataSource)
        {
            try
            {
                var cacheKey = string.Format("weather_private_{0}_{1}_{2}", endPointQueryString.ToLower(), ownWeatherDataSource.WeatherSourceId, ownWeatherDataSource.WeatherStationId);
                if (!memoryCache.TryGetValue(cacheKey, out HttpResponseMessage weatherResponse))
                {
                    var wxEndPoint = config["MicroserviceInternalCommunication:WeatherMicroservice"];
                    var url = string.Format("{0}rest/amalgamation/amalgamate/private?{1}", wxEndPoint, endPointQueryString);
                    var content = new StringContent(
                        System.Text.Json.JsonSerializer.Serialize(ownWeatherDataSource),
                        Encoding.UTF8,
                        MediaTypeNames.Application.Json);
                    weatherResponse = await httpClient.PostAsync(url, content);
                    if (weatherResponse.IsSuccessStatusCode)
                        memoryCache.Set(cacheKey, weatherResponse, MemoryCacheHelper.CreateMemoryCacheEntryOptionsMinutes(30));
                    else
                        logger.LogError(string.Format("Weather call error - GetWeatherUsingAmalgamationService. URL called: {0}. Error returned: {1}", url, await weatherResponse.Content.ReadAsStringAsync()));
                }
                return weatherResponse;
            }
            catch (Exception ex)
            {
                logger.LogError(string.Format("Error in Internal Communication - GetWeatherUsingAmalgamationService. {0}", ex.Message));
                return new HttpResponseMessage(HttpStatusCode.BadRequest);
            }
        }

        public async Task<HttpResponseMessage> GetWeatherUsingOwnService(string endPointUrl, string endPointParameters)
        {
            try
            {
                var cacheKey = string.Format("weather_own_service_{0}_{1}", endPointUrl.ToLower(), endPointParameters.ToLower());
                if (!memoryCache.TryGetValue(cacheKey, out HttpResponseMessage weatherResponse))
                {
                    weatherResponse = await httpClient.GetAsync(string.Format("{0}?{1}", endPointUrl, endPointParameters));
                    if (weatherResponse.IsSuccessStatusCode)
                        memoryCache.Set(cacheKey, weatherResponse, MemoryCacheHelper.CreateMemoryCacheEntryOptionsMinutes(30));
                }
                return weatherResponse;
            }
            catch (Exception ex)
            {
                logger.LogError(string.Format("Error in Internal Communication - GetWeatherUsingOwnService. {0}", ex.Message));
                return new HttpResponseMessage(HttpStatusCode.BadRequest);
            }
        }

        public async Task<WeatherDataSchema> GetWeatherProviderInformationFromWeatherMicroservice(string weatherId)
        {
            try
            {
                var cacheKey = string.Format("weather_{0}", weatherId.ToLower());
                if (!memoryCache.TryGetValue(cacheKey, out WeatherDataSchema weatherSchema))
                {
                    var wxEndPoint = config["MicroserviceInternalCommunication:WeatherMicroservice"];
                    var response = await httpClient.GetAsync(string.Format("{0}rest/weatherdatasource/{1}", wxEndPoint, weatherId));

                    if (!response.IsSuccessStatusCode)
                        return null;

                    var responseAsText = await response.Content.ReadAsStringAsync();
                    weatherSchema = JsonConvert.DeserializeObject<WeatherDataSchema>(responseAsText);
                    memoryCache.Set(cacheKey, weatherSchema, MemoryCacheHelper.CreateMemoryCacheEntryOptionsMinutes(30));
                }
                return weatherSchema;
            }
            catch (Exception ex)
            {
                logger.LogError(string.Format("Error in Internal Communication - GetWeatherProviderInformationFromWeatherMicroservice. {0}", ex.Message));
                return null;
            }
        }

        public async Task<HttpResponseMessage> GetWeatherUsingAmalgamationService(string endPointQueryString)
        {
            try
            {
                var cacheKey = string.Format("weather_{0}", endPointQueryString.ToLower());
                if (!memoryCache.TryGetValue(cacheKey, out HttpResponseMessage weatherResponse))
                {
                    var wxEndPoint = config["MicroserviceInternalCommunication:WeatherMicroservice"];
                    var url = string.Format("{0}rest/amalgamation/amalgamate?{1}", wxEndPoint, endPointQueryString);
                    weatherResponse = await httpClient.GetAsync(url);
                    if (weatherResponse.IsSuccessStatusCode)
                        memoryCache.Set(cacheKey, weatherResponse, MemoryCacheHelper.CreateMemoryCacheEntryOptionsMinutes(30));
                    else
                        logger.LogError(string.Format("Weather call error - GetWeatherUsingAmalgamationService. URL called: {0}. Error returned: {1}", url, await weatherResponse.Content.ReadAsStringAsync()));
                }
                return weatherResponse;
            }
            catch (Exception ex)
            {
                logger.LogError(string.Format("Error in Internal Communication - GetWeatherUsingAmalgamationService. {0}", ex.Message));
                return new HttpResponseMessage(HttpStatusCode.BadRequest);
            }
        }

        public async Task<List<int>> GetWeatherParametersAvailableByLocation(double latitude, double longitude)
        {
            try
            {
                var cacheKey = string.Format("weather_parameters_{0}_{1}", latitude.ToString(), longitude.ToString());
                if (!memoryCache.TryGetValue(cacheKey, out List<int> weatherResponse))
                {
                    var wxEndPoint = config["MicroserviceInternalCommunication:WeatherMicroservice"];
                    var url = string.Format("{0}rest/weatherparameter/location/point?latitude={1}&longitude={2}&tolerance=1000&includeFallbackParams=true&includeCalculatableParams=true",
                        wxEndPoint,
                        latitude.ToString("G", CultureInfo.InvariantCulture),
                        longitude.ToString("G", CultureInfo.InvariantCulture));
                    var httpResponse = await httpClient.GetAsync(url);
                    if (httpResponse.IsSuccessStatusCode)
                    {
                        var responseAsText = await httpResponse.Content.ReadAsStringAsync();
                        weatherResponse = JsonConvert.DeserializeObject<List<int>>(responseAsText);
                        memoryCache.Set(cacheKey, weatherResponse, MemoryCacheHelper.CreateMemoryCacheEntryOptionsMinutes(30));
                    }
                    else
                        logger.LogError(string.Format("Weather call error - GetWeatherParametersAvailableByLocation. URL called: {0}. Error returned: {1}", url, await httpResponse.Content.ReadAsStringAsync()));
                }
                return weatherResponse;
            }
            catch (Exception ex)
            {
                logger.LogError(string.Format("Error in Internal Communication - GetWeatherParametersAvailableByLocation. {0}", ex.Message));
                return null;
            }
        }

        public async Task<List<DssInformation>> GetAllListOfDssFilteredByCropsFromDssMicroservice(string cropCodes = "", string executionType = "", string country = "")
        {
            try
            {
                var language = Thread.CurrentThread.CurrentCulture.Name;
                var platformValidated = bool.Parse(this.config["AppConfiguration:DisplayNotValidatedDss"]) ? "" : "/platform_validated/true";

                var cacheKey = string.Format("listOfDss_{0}_{1}_{2}_{3}_{4}", cropCodes.ToUpper(), language.ToUpper(), executionType.ToUpper(), country.ToUpper(), platformValidated.ToUpper());
                if (!memoryCache.TryGetValue(cacheKey, out List<DssInformation> listOfDss))
                {
                    var dssEndPoint = config["MicroserviceInternalCommunication:DssMicroservice"];

                    var queryUrl = string.Format("{0}rest/dss", dssEndPoint);
                    if (!string.IsNullOrEmpty(cropCodes))
                    {
                        queryUrl = string.Format("{0}/crops/{1}", queryUrl, cropCodes);
                    }
                    var response = await httpClient.GetAsync(string.Format("{0}{1}?language={2}&executionType={3}", queryUrl, platformValidated, language, executionType.ToUpper()));

                    if (!response.IsSuccessStatusCode)
                        return listOfDss;

                    var responseAsText = await response.Content.ReadAsStringAsync();
                    listOfDss = JsonConvert.DeserializeObject<List<DssInformation>>(responseAsText);
                    memoryCache.Set(cacheKey, listOfDss, MemoryCacheHelper.CreateMemoryCacheEntryOptionsMinutes(30));
                }
                return listOfDss;
            }
            catch (Exception ex)
            {
                logger.LogError(string.Format("Error in Internal Communication - GetAllListOfDssFromDssMicroservice. {0}", ex.Message));
                return null;
            }
        }

        public async Task<List<WeatherDataSchema>> GetListWeatherProviderInformationFromWeatherMicroservice()
        {
            try
            {
                var cacheKey = string.Format("list_weather");
                if (!memoryCache.TryGetValue(cacheKey, out List<WeatherDataSchema> weatherSchema))
                {
                    var wxEndPoint = config["MicroserviceInternalCommunication:WeatherMicroservice"];
                    var response = await httpClient.GetAsync(string.Format("{0}rest/weatherdatasource", wxEndPoint));

                    if (!response.IsSuccessStatusCode)
                        return null;

                    var responseAsText = await response.Content.ReadAsStringAsync();
                    weatherSchema = JsonConvert.DeserializeObject<List<WeatherDataSchema>>(responseAsText);
                    memoryCache.Set(cacheKey, weatherSchema, MemoryCacheHelper.CreateMemoryCacheEntryOptionsMinutes(30));
                }
                return weatherSchema;
            }
            catch (Exception ex)
            {
                logger.LogError(string.Format("Error in Internal Communication - GetListWeatherProviderInformationFromWeatherMicroservice. {0}", ex.Message));
                return null;
            }
        }

        public async Task<RiskMapProvider> GetAllTheRiskMapsFromDssMicroservice()
        {
            try
            {
                var cacheKey = string.Format("list_riskmaps");
                if (!memoryCache.TryGetValue(cacheKey, out RiskMapProvider riskMapsList))
                {
                    var dssEndPoint = config["MicroserviceInternalCommunication:DssMicroservice"];
                    var response = await httpClient.GetAsync(string.Format("{0}rest/risk_maps/list ", dssEndPoint));


                    if (!response.IsSuccessStatusCode)
                        return null;

                    var responseAsText = await response.Content.ReadAsStringAsync();
                    riskMapsList = JsonConvert.DeserializeObject<RiskMapProvider>(responseAsText);
                    memoryCache.Set(cacheKey, riskMapsList, MemoryCacheHelper.CreateMemoryCacheEntryOptionsMinutes(30));
                }
                return riskMapsList;
            }
            catch (Exception ex)
            {
                logger.LogError(string.Format("Error in Internal Communication - GetAllTheRiskMapsFromDssMicroservice. {0}", ex.Message));
                return null;
            }
        }
        #endregion
    }
}