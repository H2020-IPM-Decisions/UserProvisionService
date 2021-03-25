using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace H2020.IPMDecisions.UPR.Core.Entities
{
    public class WeatherDataSource
    {
        [Key]
        public Guid Id { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string Url { get; set; }
        [Required]
        public bool IsForecast { get; set; }
        [Required]
        public bool AuthenticationRequired { get; set; }
        public int Interval { get; set; }
        public DateTime TimeStart { get; set; }
        public DateTime TimeEnd { get; set; }
        public string Credentials { get; set; }

        public Guid FarmId { get; set; }
        public Farm Farm { get; set; }

        public IList<FieldWeatherDataSource> FieldWeatherDataSources { get; set; }
    }
}