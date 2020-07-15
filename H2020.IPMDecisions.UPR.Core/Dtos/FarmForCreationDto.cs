using System.ComponentModel.DataAnnotations;
using NetTopologySuite.Geometries;

namespace H2020.IPMDecisions.UPR.Core.Dtos
{
    public class FarmForCreationDto : FarmForManipulationDto
    {
        [Required(ErrorMessage = "Name is required")]
        public override string Name { get => base.Name; set => base.Name = value; }

        [Required(ErrorMessage = "Farm location is required")]
        public override Point Location { get => base.Location; set => base.Location = value; }
    }
}