using Newtonsoft.Json;

namespace H2020.IPMDecisions.UPR.Core.Models
{
    // This class matches the schema definition of /api/dss/rest/schema/fieldobservation/nolocation - Model section
    public class DssModelFieldObservation
    {
        [JsonProperty("location")]
        public GeoJsonGeometry Location { get; set; }
        [JsonProperty("time")]
        public string Time { get; set; }
        [JsonProperty("pestEPPOCode")]
        public string PestEppoCode { get; set; }
        [JsonProperty("cropEPPOCode")]
        public string CropEppoCode { get; set; }
    }
}