using System;
using System.ComponentModel.DataAnnotations;
using H2020.IPMDecisions.UPR.Core.Validations;

namespace H2020.IPMDecisions.UPR.Core.Dtos
{
    public class WeatherDataSourceForCreationDto : WeatherDataSourceForManipulationDto
    {

        [Required]
        public override string Name { get; set; }
        [Required]
        public override string Url { get; set; }
        [Required]
        public override bool? IsForecast { get; set; }
        [Required]
        public override bool? AuthenticationRequired { get; set; }
    }
}