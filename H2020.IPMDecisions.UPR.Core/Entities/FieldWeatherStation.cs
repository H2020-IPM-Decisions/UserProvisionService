using System;

namespace H2020.IPMDecisions.UPR.Core.Entities
{
    public class FieldWeatherStation
    {
        public Guid FieldId { get; set; }
        public Field Field { get; set; }
        public Guid WeatherStationId { get; set; }
        public WeatherStation WeatherStation { get; set; }
    }
}