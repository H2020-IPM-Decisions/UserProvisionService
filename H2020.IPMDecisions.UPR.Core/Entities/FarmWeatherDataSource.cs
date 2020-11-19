using System;

namespace H2020.IPMDecisions.UPR.Core.Entities
{
    public class FarmWeatherDataSource
    {
        public Guid FarmId { get; set; }
        public Farm Farm { get; set; }
        public string WeatherDataSourceId { get; set; }
        public WeatherDataSource WeatherDataSource { get; set; }
    }
}