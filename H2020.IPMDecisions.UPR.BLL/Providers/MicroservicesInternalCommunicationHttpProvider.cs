using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using H2020.IPMDecisions.UPR.Core.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace H2020.IPMDecisions.UPR.BLL.Providers
{
    public class MicroservicesInternalCommunicationHttpProvider : IMicroservicesInternalCommunicationHttpProvider, IDisposable
    {
        private readonly HttpClient httpClient;
        private readonly IConfiguration config;
        private readonly ILogger<MicroservicesInternalCommunicationHttpProvider> logger;

        public MicroservicesInternalCommunicationHttpProvider(
            HttpClient httpClient,
            IConfiguration config,
            ILogger<MicroservicesInternalCommunicationHttpProvider> logger)
        {
            this.httpClient = httpClient ?? throw new System.ArgumentNullException(nameof(httpClient));
            this.config = config ?? throw new System.ArgumentNullException(nameof(config));
            this.logger = logger ?? throw new System.ArgumentNullException(nameof(logger));
        }

        public void Dispose()
        {
            httpClient?.Dispose();
        }

        public async Task<DssModelInformation> GetDssModelInformationFromDssMicroservice(string dssId, string modelId)
        {
            try
            {
                var dssEndPoint = config["MicroserviceInternalCommunication:DssMicroservice"];
                var response = await httpClient.GetAsync(string.Format("{0}rest/model/{1}/{2}", dssEndPoint, dssId, modelId));

                if (response.IsSuccessStatusCode)
                {
                    var responseAsText = await response.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<DssModelInformation>(responseAsText);
                }
                return null;
            }
            catch (Exception ex)
            {
                logger.LogError(string.Format("Error in Internal Communication - GetDssInformationFromDssMicroservice. {0}", ex.Message));
                return null;
            }
        }

        public async Task<string> GetUserIdFromIdpMicroservice(string userEmail)
        {
            try
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

                if (userIdResponse.IsSuccessStatusCode)
                {
                    return await userIdResponse.Content.ReadAsStringAsync();
                }
                return null;
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

        public async Task<HttpResponseMessage> GetWeatherUsingAmalgamationService(string endPointUrl, string endPointQueryString)
        {
            try
            {
                var wxEndPoint = config["MicroserviceInternalCommunication:WeatherMicroservice"];
                var endPointUrlEncoded = HttpUtility.UrlEncode(endPointUrl);
                var endPointQueryStringEncoded = HttpUtility.UrlEncode(endPointQueryString);

                return await httpClient.GetAsync(string.Format("{0}rest/amalgamation/amalgamate/?endpointURL={1}&endpointQueryStr={2}", wxEndPoint, endPointUrlEncoded, endPointQueryStringEncoded));
            }
            catch (Exception ex)
            {
                logger.LogError(string.Format("Error in Internal Communication - GetWeatherUsingAmalgamationService. {0}", ex.Message));
                return new HttpResponseMessage(HttpStatusCode.BadRequest);
            }
        }

        public async Task<IEnumerable<DssInformation>> GetAllListOfDssFromDssMicroservice()
        {
            try
            {
                var dssEndPoint = config["MicroserviceInternalCommunication:DssMicroservice"];
                var response = await httpClient.GetAsync(string.Format("{0}rest/dss", dssEndPoint));

                if (response.IsSuccessStatusCode)
                {
                    var responseAsText = await response.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<IEnumerable<DssInformation>>(responseAsText);
                }
                return null;
            }
            catch (Exception ex)
            {
                logger.LogError(string.Format("Error in Internal Communication - GetAllListOfDssFromDssMicroservice. {0}", ex.Message));
                return null;
            }
        }

        public async Task<WeatherDataSchema> GetWeatherProviderInformationFromWeatherMicroservice(string weatherId)
        {
            try
            {
                var wxEndPoint = config["MicroserviceInternalCommunication:WeatherMicroservice"];
                var response = await httpClient.GetAsync(string.Format("{0}rest/weatherdatasource/{1}", wxEndPoint, weatherId));

                if (response.IsSuccessStatusCode)
                {
                    var responseAsText = await response.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<WeatherDataSchema>(responseAsText);
                }
                return null;
            }
            catch (Exception ex)
            {
                logger.LogError(string.Format("Error in Internal Communication - GetWeatherProviderInformationFromWeatherMicroservice. {0}", ex.Message));
                return null;
            }
        }
    }
}