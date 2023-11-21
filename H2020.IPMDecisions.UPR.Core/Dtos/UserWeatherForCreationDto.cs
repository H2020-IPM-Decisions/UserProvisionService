using System.ComponentModel.DataAnnotations;

namespace H2020.IPMDecisions.UPR.Core.Dtos
{
    public class UserWeatherForCreationDto
    {
        [Required]
        public string UserWeatherStationId { get; set; }
        public string UserWeatherStationReference { get; set; }
        [Required]
        public string UserWeatherUsername { get; set; }
        [Required]
        public string UserWeatherPassword { get; set; }
    }
}