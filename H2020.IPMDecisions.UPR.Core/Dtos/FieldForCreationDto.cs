using System.ComponentModel.DataAnnotations;

namespace H2020.IPMDecisions.UPR.Core.Dtos
{
    public class FieldForCreationDto : FieldForManipulationDto
    {
        [Required(ErrorMessage = "Name is required")]
        public override string Name { get => base.Name; set => base.Name = value; }
    }
}