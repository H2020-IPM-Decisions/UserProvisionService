using System.Collections.Generic;
using Newtonsoft.Json;

namespace H2020.IPMDecisions.UPR.Core.Models
{
    public class WeatherAdapterBodyRequest
    {
        [JsonProperty("credentials")]
        public AdapterCredentials Credentials { get; set; } = new();
        [JsonProperty("weatherStationId")]
        public string WeatherStationId { get; set; }
        [JsonProperty("interval")]
        public string Interval { get; set; } = "3600";
        [JsonProperty("timeStart")]
        public string TimeStart { get; set; }
        [JsonProperty("timeEnd")]
        public string TimeEnd { get; set; }
        [JsonProperty("parameters")]
        public string Parameters { get; set; } = "1002";

        public List<KeyValuePair<string, string>> ToKeyValuePairList()
        {
            var keyValuePairs = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("weatherStationId", WeatherStationId),
                new KeyValuePair<string, string>("interval", Interval),
                new KeyValuePair<string, string>("timeStart", TimeStart),
                new KeyValuePair<string, string>("timeEnd", TimeEnd),
                new KeyValuePair<string, string>("parameters", Parameters),
                new KeyValuePair<string, string>("credentials", JsonConvert.SerializeObject(Credentials))
            };
            return keyValuePairs;
        }
    }

    public class AdapterCredentials
    {
        [JsonProperty("userName")]
        public string UserName { get; set; }
        [JsonProperty("password")]
        public string Password { get; set; }
    }
}