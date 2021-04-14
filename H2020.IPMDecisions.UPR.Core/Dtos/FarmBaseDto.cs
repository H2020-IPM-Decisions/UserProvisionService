using H2020.IPMDecisions.UPR.Core.Models;
using System;

namespace H2020.IPMDecisions.UPR.Core.Dtos
{
    public abstract class FarmBaseDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Inf1 { get; set; }
        public string Inf2 { get; set; }
        public CustomPointLocation Location { get; set; }
        public WeatherHistoricalDto WeatherHistoricalDto { get; set; }
        public WeatherForecastDto WeatherForecastDto { get; set; }
    }
}