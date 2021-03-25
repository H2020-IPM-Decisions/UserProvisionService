using System.ComponentModel.DataAnnotations;
using H2020.IPMDecisions.UPR.Core.Models;

namespace H2020.IPMDecisions.UPR.Core.Dtos
{
    public class FarmForUpdateDto : FarmForManipulationDto
    {
        [Required(ErrorMessage = "Name can not be deleted")]
        public override string Name { get => base.Name; set => base.Name = value; }

        [Required(ErrorMessage = "Farm location can not be deleted")]
        public override CustomPointLocation Location { get => base.Location; set => base.Location = value; }

        [Required(ErrorMessage = "Weather Station can not be deleted")]
        public WeatherStationForUpdateDto WeatherStationDto { get; set; }

        [Required(ErrorMessage = "Weather Data Source can not be deleted")]
        public WeatherDataSourceForUpdateDto WeatherDataSourceDto { get; set; }
    }
}