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
        Task<UserInformationInternalCall> GetUserInformationFromIdpMicroservice(string userEmail);
        Task<bool> SendDataRequestEmail(string requesterEmail, string toEmail);
        Task<DssModelInformation> GetDssModelInformationFromDssMicroservice(string dssId, string modelId);
        Task<JSchema> GetDssModelInputSchemaMicroservice(string dssId, string modelId);
        Task<bool> ValidateWeatherDataSchemaFromDssMicroservice(string weatherDataSchema);
        Task<HttpResponseMessage> GetWeatherUsingAmalgamationPrivateService(string endPointQueryString, PrivateWeatherBodyRequest ownWeatherDataSource);
        Task<HttpResponseMessage> GetWeatherUsingAmalgamationService(string endPointQueryString);
        Task<HttpResponseMessage> GetWeatherUsingOwnService(string endPointUrl, string endPointParameters);
        Task<bool> ValidateLoginDetailPersonaWeatherStation(string endPointQueryString, List<KeyValuePair<string, string>> collection);
        Task<IEnumerable<DssInformation>> GetAllListOfDssFromDssMicroservice();
        Task<List<WeatherDataSchema>> GetListWeatherProviderInformationFromWeatherMicroservice();
        Task<WeatherDataSchema> GetWeatherProviderInformationFromWeatherMicroservice(string weatherId);
        Task<List<string>> GetListOfEppoCodesFromDssMicroservice(string eppoCodeType, string executionType);
        Task<IEnumerable<DssInformation>> GetListOfDssByLocationFromDssMicroservice(GeoJsonFeatureCollection geoJson, string executionType = "");
        Task<DssInformation> GetDssInformationFromDssMicroservice(string dssId);
        Task<List<int>> GetWeatherParametersAvailableByLocation(double latitude, double longitude);
        Task<List<DssInformation>> GetAllListOfDssFilteredByCropsFromDssMicroservice(string cropCodes = "", string executionType = "", string country = "");
        Task<RiskMapProvider> GetAllTheRiskMapsFromDssMicroservice();
    }
}