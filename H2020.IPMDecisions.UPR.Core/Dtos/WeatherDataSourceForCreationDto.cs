using System.ComponentModel.DataAnnotations;

namespace H2020.IPMDecisions.UPR.Core.Dtos
{
    public class WeatherDataSourceForCreationDto : WeatherForManipulationDto
    {
        [Required]
        public override string WeatherId { get; set; }
    }
}