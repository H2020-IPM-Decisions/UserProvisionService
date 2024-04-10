using System.Collections.Generic;
using System.Linq;
using H2020.IPMDecisions.UPR.Core.Dtos;
using Newtonsoft.Json;

namespace H2020.IPMDecisions.UPR.Core.Models
{
    // This class matches the schema definition of /api/dss/apidocs/json_RiskMapProvider.html
    public class RiskMapProvider
    {
        [JsonProperty("risk_map_providers")]
        public List<RiskMapProviders> RiskMapProviders { get; set; }

        public List<RiskMapFullDetailDto> ToRiskMapBaseDto()
        {
            return RiskMapProviders
            .SelectMany(provider => provider.RiskMaps
                .Select(riskMap => new RiskMapFullDetailDto
                {
                    Id = riskMap.Id,
                    Title = riskMap.Title,
                    WmsUrl = riskMap.WmsUrl,
                    PlatformValidated = riskMap.PlatformValidated,
                    ProviderId = provider.Id,
                    ProviderName = provider.Name,
                    ProviderCountry = provider.City,
                    ProviderAddress = provider.Address,
                    ProviderPostalCode = provider.PostalCode,
                    ProviderCity = provider.City,
                    ProviderUrl = provider.Url,
                }))
            .ToList();
        }
    }

    public class RiskMapProviders
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Country { get; set; }
        public string Address { get; set; }
        [JsonProperty("postal_code")]
        public string PostalCode { get; set; }
        public string City { get; set; }
        public string Email { get; set; }
        public string Url { get; set; }
        [JsonProperty("risk_maps")]
        public List<RiskMap> RiskMaps { get; set; }
    }

    public class RiskMap
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public string Address { get; set; }
        [JsonProperty("wms_url")]
        public string WmsUrl { get; set; }
        [JsonProperty("platform_validated")]
        public string PlatformValidated { get; set; }
    }
}