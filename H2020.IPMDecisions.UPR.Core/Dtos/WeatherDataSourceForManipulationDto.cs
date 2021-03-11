using System;
using System.ComponentModel.DataAnnotations;
using H2020.IPMDecisions.UPR.Core.Validations;

namespace H2020.IPMDecisions.UPR.Core.Dtos
{
    public abstract class WeatherDataSourceForManipulationDto
    {
        public abstract string Name { get; set; }
        [DataType(DataType.Url)]
        public abstract string Url { get; set; }
        public abstract bool? IsForecast { get; set; }
        public abstract bool? AuthenticationRequired { get; set; }
        public string StationId { get; set; }

        [NotWeatherForecastAttribute]
        public int Interval { get; set; }

        [NotWeatherForecastAttribute]
        public DateTime? TimeStart { get; set; }

        [NotWeatherForecastAttribute]
        public DateTime? TimeEnd { get; set; }

        [NotWeatherForecastAttribute]
        public string Parameters { get; set; }

        [WeatherDataAuthRequiredAttribute]
        public WeatherCredentials Credentials { get; set; }
    }
}