using System.ComponentModel.DataAnnotations;

namespace H2020.IPMDecisions.UPR.Core.Dtos
{
    public class WeatherStationForCreationDto : WeatherForManipulationDto
    {

        [Required]
        public override string Name { get; set; }
        [Required]
        public override string Url { get; set; }
        [Required]
        public override bool? IsForecast { get; set; }
        [Required]
        public override bool? AuthenticationRequired { get; set; }
        [Required]
        public string WeatherStationId { get; set; }
    }
}