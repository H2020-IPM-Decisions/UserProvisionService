using System;

namespace H2020.IPMDecisions.UPR.Core.Dtos
{
    public class WeatherForecastDto
    {
        public Guid Id { get; set; }
        public string WeatherId { get; set; }
        public string Name { get; set; }
        public string Url { get; set; }
    }
}