using System;

namespace H2020.IPMDecisions.UPR.Core.Entities
{
    public class FieldWeatherDataSource
    {
        public Guid FieldId { get; set; }
        public Field Field { get; set; }
        public Guid WeatherDataSourceId { get; set; }
        public WeatherDataSource WeatherDataSource { get; set; }
    }
}