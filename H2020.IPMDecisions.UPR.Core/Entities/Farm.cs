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
        public string Inf1 { get; set; }
        public string Inf2 { get; set; }
        public Point Location { get; set; }

        public IList<UserFarm> UserFarms { get; set; }
        public IList<FarmWeatherStation> FarmWeatherStations { get; set; }
        public IList<WeatherDataSource> FarmWeatherDataSources { get; set; }

        public ICollection<Field> Fields { get; set; }
    }
}