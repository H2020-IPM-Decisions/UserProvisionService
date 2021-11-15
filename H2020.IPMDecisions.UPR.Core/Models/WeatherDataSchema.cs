using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace H2020.IPMDecisions.UPR.Core.Models
{
    // This class matches the schema definition of /api/wx/rest/schema/weatherdata
    public class WeatherDataSchema
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public Uri EndPoint { get; set; }
        [JsonProperty("authentication_required")]
        public bool AuthenticationRequired { get; set; }
        [JsonProperty("access_type")]
        public string AccessType { get; set; }
        public WeatherDataTemporal Temporal { get; set; }
        public WeatherDataParameters Parameters { get; set; }
    }

    public class WeatherDataParameters
    {
        public IEnumerable<int> Common { get; set; }
        public IEnumerable<int> Optional { get; set; }
    }

    public class WeatherDataTemporal
    {
        public int Forecast { get; set; }
        public WeatherTemporalHistoric Historic { get; set; }
    }

    public class WeatherTemporalHistoric
    {
        public string Start { get; set; }
        public string End { get; set; }
    }
}