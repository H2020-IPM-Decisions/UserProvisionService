using System;
using System.Collections.Generic;

namespace H2020.IPMDecisions.UPR.Core.Dtos
{
    public class UserWeatherDto
    {
        public Guid Id { get; set; }
        public string WeatherId { get; set; }
        public string WeatherName { get; set; }
        public string WeatherStationId { get; set; }
        public string WeatherStationReference { get; set; }
        public string UserName { get; set; }
        public List<FarmDto> Farms { get; set; }
    }
}