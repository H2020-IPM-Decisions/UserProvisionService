using System;

namespace H2020.IPMDecisions.UPR.Core.Dtos
{
    public class UserWeatherDto
    {
        public Guid Id { get; set; }
        public string WeatherId { get; set; }
        public string WeatherDataSourceId { get; set; }
        public string WeatherStationId { get; set; }
        public string WeatherStationName { get; set; }
        public string WeatherStationReference { get; set; }
        public string UserName { get; set; }
    }
}