using System;

namespace H2020.IPMDecisions.UPR.Core.Dtos
{
    public class WeatherDataSourceDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Url { get; set; }
        public bool IsForecast { get; set; }
        public bool AuthenticationRequired { get; set; }
        public int Interval { get; set; }
        public DateTime TimeStart { get; set; }
        public DateTime TimeEnd { get; set; }
        public string Parameters { get; set; }
        public string Credentials { get; set; }
    }
}