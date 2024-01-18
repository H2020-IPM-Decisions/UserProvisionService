using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using NetTopologySuite.Geometries;

namespace H2020.IPMDecisions.UPR.Core.Entities
{
    public class Farm
    {
        [Key]
        public Guid Id { get; set; }
        [Required]
        public string Name { get; set; }
        public Point Location { get; set; }
        public Guid WeatherForecastId { get; set; }
        public WeatherForecast WeatherForecast { get; set; }
        public Guid WeatherHistoricalId { get; set; }
        public WeatherHistorical WeatherHistorical { get; set; }
        public Guid? UserWeatherId { get; set; }
        public UserWeather UserWeather { get; set; }

        public IList<UserFarm> UserFarms { get; set; }
        public ICollection<Field> Fields { get; set; }
    }
}