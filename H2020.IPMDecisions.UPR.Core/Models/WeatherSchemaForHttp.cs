using System;

namespace H2020.IPMDecisions.UPR.Core.Models
{
    public class WeatherSchemaForHttp
    {
        public string WeatherId { get; set; }
        public string Url { get; set; }
        public bool IsForecast { get; set; }
        public int Interval { get; set; }
        public DateTime WeatherTimeStart { get; set; }
        public DateTime WeatherTimeEnd { get; set; }
        public string WeatherDssParameters { get; set; }
    }
}