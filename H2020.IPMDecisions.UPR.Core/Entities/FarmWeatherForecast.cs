using System;

namespace H2020.IPMDecisions.UPR.Core.Entities
{
    public class FarmWeatherForecast
    {
        public Guid FarmId { get; set; }
        public Farm Farm { get; set; }
        public Guid WeatherForecastId { get; set; }
        public WeatherForecast WeatherForecast { get; set; }
    }
}