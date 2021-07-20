using System.Collections.Generic;
using Newtonsoft.Json;

namespace H2020.IPMDecisions.UPR.Core.Models
{
    // This class matches the schema definition of /api/dss/rest/schema/dss
    public class DssInformation
    {
        public string Id { get; set; }
        public string Version { get; set; }
        public string Name { get; set; }
        public string Url { get; set; }
        public List<string> Languages { get; set; }
        [JsonProperty("organization")]
        public DssOrganization DssOrganization { get; set; }
        [JsonProperty("models")]
        public IEnumerable<DssModelInformation> DssModelInformation { get; set; }
    }

    public class DssOrganization
    {
        public string Name { get; set; }
        public string Country { get; set; }
        public string Address { get; set; }
        [JsonProperty("postal_code")]
        public string PostalCode { get; set; }
        public string City { get; set; }
        public string Email { get; set; }
        public string Url { get; set; }
    }
}