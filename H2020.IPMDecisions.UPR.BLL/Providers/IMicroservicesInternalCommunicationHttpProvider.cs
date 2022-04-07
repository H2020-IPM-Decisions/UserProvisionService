using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using H2020.IPMDecisions.UPR.Core.Models;
using Newtonsoft.Json.Schema;

namespace H2020.IPMDecisions.UPR.BLL.Providers
{
    public interface IMicroservicesInternalCommunicationHttpProvider
    {
        Task<string> GetUserIdFromIdpMicroservice(string userEmail);
        Task<bool> SendDataRequestEmail(string requesterEmail, string toEmail);
        Task<DssModelInformation> GetDssModelInformationFromDssMicroservice(string dssId, string modelId);
        Task<JSchema> GetDssModelInputSchemaMicroservice(string dssId, string modelId);
        Task<bool> ValidateWeatherdDataSchemaFromDssMicroservice(string weatherDataSchema);
        Task<HttpResponseMessage> GetWeatherUsingAmalgamationProxyService(string endPointUrl, string endPointQueryString);
        Task<HttpResponseMessage> GetWeatherUsingAmalgamationService(string endPointQueryString);
        Task<HttpResponseMessage> GetWeatherUsingOwnService(string endPointUrl, string endPointParameters);
        Task<IEnumerable<DssInformation>> GetAllListOfDssFromDssMicroservice();
        Task<WeatherDataSchema> GetWeatherProviderInformationFromWeatherMicroservice(string weatherId);
        Task<List<string>> GetListOfEppoCodesFromDssMicroservice(string eppoCodeType);
        Task<IEnumerable<DssInformation>> GetListOfDssByLocationFromDssMicroservice(GeoJsonFeatureCollection geoJson);
    }
}