using System.ComponentModel.DataAnnotations;

namespace H2020.IPMDecisions.UPR.Core.Dtos
{
    public class AdminVariableForManipulationDto
    {
        [Required]
        public string Description { get; set; }
        [Required]
        public string Value { get; set; }
    }
}