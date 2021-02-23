using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace H2020.IPMDecisions.UPR.Core.Entities
{
    public class WeatherStation
    {
        [Key]
        public string Id { get; set; }
        public string Name { get; set; }

        public IList<FarmWeatherStation> FarmWeatherStations { get; set; }
        public IList<FieldWeatherStation> FieldWeatherStations { get; set; }
    }
}