using System.ComponentModel.DataAnnotations;

namespace H2020.IPMDecisions.UPR.Core.Dtos
{
    public class WeatherServiceForCreationDto
    {
        [Required]
        public string WeatherId { get; set; }
    }
}