using System;
using System.ComponentModel.DataAnnotations;
using H2020.IPMDecisions.UPR.Core.Validations;

namespace H2020.IPMDecisions.UPR.Core.Dtos
{
    public class WeatherDataSourceForCreationDto
    {
        [Required]
        public string Name { get; set; }
        [Required]
        [DataType(DataType.Url)]
        public string Url { get; set; }
        [Required]
        public bool? IsForecast { get; set; }
        [Required]
        public bool? AuthenticationRequired { get; set; }
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