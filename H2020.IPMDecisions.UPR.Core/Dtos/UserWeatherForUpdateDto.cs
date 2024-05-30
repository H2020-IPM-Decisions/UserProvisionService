using System.ComponentModel.DataAnnotations;

namespace H2020.IPMDecisions.UPR.Core.Dtos
{
    public class UserWeatherForUpdateDto
    {
        [Required]
        public string WeatherStationId { get; set; }
        public string WeatherStationReference { get; set; }
        [Required]
        public string UserName { get; set; }
        [Required]
        public string Password { get; set; }
    }
}