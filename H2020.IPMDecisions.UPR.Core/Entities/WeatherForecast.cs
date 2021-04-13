using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace H2020.IPMDecisions.UPR.Core.Entities
{
    public class WeatherForecast
    {
        [Key]
        public Guid Id { get; set; }
        [Required]
        public Guid WeatherId { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string Url { get; set; }

        public ICollection<FarmWeatherForecast> FarmWeatherForecast { get; set; }
    }
}