using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace H2020.IPMDecisions.UPR.Core.Entities
{
    public class WeatherDataSource
    {
        [Key]
        public string Id { get; set; }
        public string Name { get; set; }

        public IList<FarmWeatherDataSource> FarmWeatherDataSources { get; set; }
        public IList<FieldWeatherDataSource> FieldWeatherDataSources { get; set; }
    }
}