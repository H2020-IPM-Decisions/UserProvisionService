using System.ComponentModel.DataAnnotations;

namespace H2020.IPMDecisions.UPR.Core.Dtos
{
    public class UserWeatherForCreationDto
    {

        [Required]
        public string WeatherDataSourceId { get; set; }
        [Required]
        public string WeatherStationId { get; set; }
        public string WeatherStationReference { get; set; }
        [Required]
        public string WeatherUsername { get; set; }
        [Required]
        public string WeatherPassword { get; set; }
    }
}