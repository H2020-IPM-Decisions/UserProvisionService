using System.ComponentModel.DataAnnotations;
using H2020.IPMDecisions.UPR.Core.Models;

namespace H2020.IPMDecisions.UPR.Core.Dtos
{
    public class FarmForCreationDto : FarmForManipulationDto
    {
        [Required(ErrorMessage = "Name is required")]
        public override string Name { get => base.Name; set => base.Name = value; }

        [Required(ErrorMessage = "Farm location is required")]
        public override CustomPointLocation Location { get => base.Location; set => base.Location = value; }

        [Required(ErrorMessage = "Weather Station is required")]
        public override WeatherStationDto WeatherStationDto { get => base.WeatherStationDto; set => base.WeatherStationDto = value; }

        [Required(ErrorMessage = "Weather Data Source is required")]
        public override WeatherDataSourceDto WeatherDataSourceDto { get => base.WeatherDataSourceDto; set => base.WeatherDataSourceDto = value; }

    }
}