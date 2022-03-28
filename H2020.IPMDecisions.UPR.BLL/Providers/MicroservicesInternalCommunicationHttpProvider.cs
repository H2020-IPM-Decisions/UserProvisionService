using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Mime;
using System.Text;
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
        // ToDo Ask for DSS languages
        public async Task<DssModelInformation> GetDssModelInformationFromDssMicroservice(string dssId, string modelId)
        {
            try
            {
                var cacheKey = string.Format("dssInformation_{0}_{1}", dssId.ToLower(), modelId.ToLower());
                if (!memoryCache.TryGetValue(cacheKey, out DssModelInformation dssModelInformation))
                {
                    var dssEndPoint = config["MicroserviceInternalCommunication:DssMicroservice"];
                    var response = await httpClient.GetAsync(string.Format("{0}rest/model/{1}/{2}", dssEndPoint, dssId, modelId));

                    if (!response.IsSuccessStatusCode) return null;

                    var responseAsText = await response.Content.ReadAsStringAsync();
                    dssModelInformation = JsonConvert.DeserializeObject<DssModelInformation>(responseAsText);

                    memoryCache.Set(cacheKey, dssModelInformation, MemoryCacheHelper.CreateMemoryCacheEntryOptions(1));
                }
                return dssModelInformation;
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
                var cacheKey = "listOfDss";
                if (!memoryCache.TryGetValue(cacheKey, out IEnumerable<DssInformation> listOfDss))
                {
                    var dssEndPoint = config["MicroserviceInternalCommunication:DssMicroservice"];
                    var response = await httpClient.GetAsync(string.Format("{0}rest/dss", dssEndPoint));

                    if (!response.IsSuccessStatusCode)
                        return null;

                    var responseAsText = await response.Content.ReadAsStringAsync();
                    listOfDss = JsonConvert.DeserializeObject<IEnumerable<DssInformation>>(responseAsText);
                    memoryCache.Set(cacheKey, listOfDss, MemoryCacheHelper.CreateMemoryCacheEntryOptions(1));
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
                var cacheKey = string.Format("dssInputSchema_{0}_{1}", dssId.ToLower(), modelId.ToLower());
                if (!memoryCache.TryGetValue(cacheKey, out JSchema dssInputSchema))
                {
                    var dssEndPoint = config["MicroserviceInternalCommunication:DssMicroservice"];
                    var response = await httpClient.GetAsync(string.Format("{0}rest/model/{1}/{2}/input_schema/ui_form", dssEndPoint, dssId, modelId));

                    if (!response.IsSuccessStatusCode) return null;

                    var responseAsString = await response.Content.ReadAsStringAsync();
                    JSchemaUrlResolver resolver = new JSchemaUrlResolver();
                    dssInputSchema = JSchema.Parse(responseAsString, resolver);
                    // Dss for caching as the previous one will have some properties removed.
                    var dssInputSchemaCached = JSchema.Parse(responseAsString, resolver);
                    memoryCache.Set(cacheKey, dssInputSchemaCached, MemoryCacheHelper.CreateMemoryCacheEntryOptions(1));
                }
                return dssInputSchema;
            }
            catch (Exception ex)
            {
                logger.LogError(string.Format("Error in Internal Communication - GetDssModelInputSchemaMicroservice. {0}", ex.Message));
                return null;
            }
        }

        public async Task<List<string>> GetListOfEppoCodesFromDssMicroservice(string eppoCodeType)
        {
            try
            {
                var cacheKey = string.Format("eppoCodes_{0}", eppoCodeType.ToLower());
                if (!memoryCache.TryGetValue(cacheKey, out List<string> listOfEppoCodes))
                {
                    var dssEndPoint = config["MicroserviceInternalCommunication:DssMicroservice"];
                    var response = await httpClient.GetAsync(string.Format("{0}rest/{1}", dssEndPoint, eppoCodeType));

                    if (!response.IsSuccessStatusCode) return null;

                    var responseAsText = await response.Content.ReadAsStringAsync();
                    listOfEppoCodes = JsonConvert.DeserializeObject<List<string>>(responseAsText);

                    memoryCache.Set(cacheKey, listOfEppoCodes, MemoryCacheHelper.CreateMemoryCacheEntryOptions(1));
                }
                return listOfEppoCodes;
            }
            catch (Exception ex)
            {
                logger.LogError(string.Format("Error in Internal Communication - GetListOfEppoCodesFromDssMicroservice. {0}", ex.Message));
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
                    memoryCache.Set(cacheKey, userId, MemoryCacheHelper.CreateMemoryCacheEntryOptions(15));
                }
                return userId;
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

        public async Task<bool> ValidateWeatherdDataSchemaFromDssMicroservice(string weatherDataSchema)
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

        public async Task<HttpResponseMessage> GetWeatherUsingAmalgamationProxyService(string endPointUrl, string endPointQueryString)
        {
            try
            {
                var cacheKey = string.Format("weather_{0}_{1}", endPointUrl.ToLower(), endPointQueryString.ToLower());
                if (!memoryCache.TryGetValue(cacheKey, out HttpResponseMessage weatherResponse))
                {
                    var wxEndPoint = config["MicroserviceInternalCommunication:WeatherMicroservice"];
                    var endPointUrlEncoded = HttpUtility.UrlEncode(endPointUrl);
                    var endPointQueryStringEncoded = HttpUtility.UrlEncode(endPointQueryString);
                    var url = string.Format("{0}rest/amalgamation/amalgamate/proxy/?endpointURL={1}&endpointQueryStr={2}", wxEndPoint, endPointUrlEncoded, endPointQueryStringEncoded);
                    weatherResponse = await httpClient.GetAsync(url);
                    if (weatherResponse.IsSuccessStatusCode)
                        memoryCache.Set(cacheKey, weatherResponse, MemoryCacheHelper.CreateMemoryCacheEntryOptions(1));
                    else
                        logger.LogError(string.Format("Weather call error. URL called: {0}. Error returned: {1}", url, await weatherResponse.Content.ReadAsStringAsync()));
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
                        memoryCache.Set(cacheKey, weatherResponse, MemoryCacheHelper.CreateMemoryCacheEntryOptions(1));
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
                    memoryCache.Set(cacheKey, weatherSchema, MemoryCacheHelper.CreateMemoryCacheEntryOptions(1));
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
                        memoryCache.Set(cacheKey, weatherResponse, MemoryCacheHelper.CreateMemoryCacheEntryOptions(1));
                    else
                        logger.LogError(string.Format("Weather call error. URL called: {0}. Error returned: {1}", url, await weatherResponse.Content.ReadAsStringAsync()));
                }
                return weatherResponse;
            }
            catch (Exception ex)
            {
                logger.LogError(string.Format("Error in Internal Communication - GetWeatherUsingAmalgamationService. {0}", ex.Message));
                return new HttpResponseMessage(HttpStatusCode.BadRequest);
            }
        }
        #endregion
    }
}