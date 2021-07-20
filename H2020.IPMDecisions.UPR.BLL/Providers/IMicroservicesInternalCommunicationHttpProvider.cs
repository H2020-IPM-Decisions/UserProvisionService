using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using H2020.IPMDecisions.UPR.Core.Models;

namespace H2020.IPMDecisions.UPR.BLL.Providers
{
    public interface IMicroservicesInternalCommunicationHttpProvider
    {
        Task<string> GetUserIdFromIdpMicroservice(string userEmail);
        Task<bool> SendDataRequestEmail(string requesterEmail, string toEmail);
        Task<DssModelInformation> GetDssModelInformationFromDssMicroservice(string dssId, string modelId);
        Task<bool> ValidateWeatherdDataSchemaFromDssMicroservice(string weatherDataSchema);
        Task<HttpResponseMessage> GetWeatherUsingAmalgamationService(string endPointUrl, string endPointQueryString);
        Task<IEnumerable<DssInformation>> GetAllListOfDssFromDssMicroservice();
    }
}