using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace H2020.IPMDecisions.UPR.Core.Models
{
    // This class matches the schema definition of /api/wx/rest/schema/weatherdata
    public class WeatherDataResponseSchema
    {
        [JsonProperty("timeStart")]
        public DateTime TimeStart { get; set; }
        [JsonProperty("timeEnd")]
        public DateTime TimeEnd { get; set; }
        [JsonProperty("interval")]
        public int Interval { get; set; }
        [JsonProperty("weatherParameters")]
        public List<int> WeatherParameters { get; set; }
        [JsonProperty("locationWeatherData")]
        public List<LocationWeatherDataResult> LocationWeatherDataResult { get; set; }
    }

    public class LocationWeatherDataResult
    {
        [JsonProperty("longitude")]
        public double? Longitude { get; set; }
        [JsonProperty("latitude")]
        public double? Latitude { get; set; }
        [JsonProperty("altitude")]
        public double? Altitude { get; set; }
        [JsonProperty("amalgamation")]
        public List<int?> Amalgamation { get; set; }
        [JsonProperty("width")]
        public int Width { get; set; }
        [JsonProperty("qc")]
        public List<int?> Qc { get; set; }
        [JsonProperty("length")]
        public int Length { get; set; }
        [JsonProperty("data")]
        public List<List<double?>> Data { get; set; }
    }
}