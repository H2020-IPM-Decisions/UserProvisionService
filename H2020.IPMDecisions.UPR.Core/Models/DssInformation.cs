using System.Collections.Generic;
using Newtonsoft.Json;

namespace H2020.IPMDecisions.UPR.Core.Models
{
    // This class matches the schema definition of /api/dss/rest/schema/dss
    public class DssInformation
    {
        [JsonProperty("models")]
        public IEnumerable<DssModelInformation> DssModelInformation { get; set; }        
    }
}