using System;

namespace H2020.IPMDecisions.UPR.Core.Models
{
    public class WeatherSchemaForHttp
    {
        public string Url { get; set; }
        public bool IsForecast { get; set; }
        public bool AuthenticationRequired { get; set; }
        public string StationId { get; set; }
        public int Interval { get; set; }
        public DateTime TimeStart { get; set; }
        public DateTime TimeEnd { get; set; }
        public string Credentials { get; set; }
    }
}