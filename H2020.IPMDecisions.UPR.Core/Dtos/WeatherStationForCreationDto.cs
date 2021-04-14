using System.ComponentModel.DataAnnotations;

namespace H2020.IPMDecisions.UPR.Core.Dtos
{
    public class WeatherStationForCreationDto : WeatherForManipulationDto
    {
        public override string WeatherId { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string Url { get; set; }
        [Required]
        public bool? IsForecast { get; set; }
        [Required]
        public bool? AuthenticationRequired { get; set; }
        [Required]
        public string WeatherStationId { get; set; }
    }
}