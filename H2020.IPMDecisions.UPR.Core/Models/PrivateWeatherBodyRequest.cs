using System.Text.Json.Serialization;

namespace H2020.IPMDecisions.UPR.Core.Models
{
    public class PrivateWeatherBodyRequest
    {
        [JsonPropertyName("userName")]
        public string UserName { get; set; }
        [JsonPropertyName("password")]
        public string Password { get; set; }
        [JsonPropertyName("weatherSourceId")]
        public string WeatherSourceId { get; set; }
        [JsonPropertyName("weatherStationId")]
        public string WeatherStationId { get; set; }
    }
}