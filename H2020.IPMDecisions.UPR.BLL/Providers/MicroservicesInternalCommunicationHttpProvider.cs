using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using H2020.IPMDecisions.UPR.Core.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
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

        public async Task<DssExecutionInformation> GetDssInformationFromDssMicroservice(string dssId, string modelId)
        {
            try
            {
                var dssEndPoint = config["MicroserviceInternalCommunication:DssMicroservice"];
                var response = await httpClient.GetAsync(string.Format("{0}rest/model/{1}/{2}", dssEndPoint, dssId, modelId));

                if (response.IsSuccessStatusCode)
                {
                    var responseAsText = await response.Content.ReadAsStringAsync();
                    JObject jObject = JObject.Parse(responseAsText);

                    var dssInformation = new DssExecutionInformation()
                    {
                        EndPoint = jObject["execution"]["endpoint"].ToString(),
                        Type = jObject["execution"]["type"].ToString()
                    };

                    var inputSchema = jObject["execution"]["input_schema"].ToString();
                    if (!string.IsNullOrEmpty(inputSchema))
                    {
                        JObject inputSchemaObject = JObject.Parse(inputSchema);
                        if (inputSchemaObject["properties"]["weatherData"] != null)
                        {
                            dssInformation.UsesWeatherData = true;
                        }
                    }
                    return dssInformation;
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
    }
}