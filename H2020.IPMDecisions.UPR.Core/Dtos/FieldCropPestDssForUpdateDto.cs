using System.ComponentModel.DataAnnotations;

namespace H2020.IPMDecisions.UPR.Core.Dtos
{
    public class FieldCropPestDssForUpdateDto
    {
        [Required]
        public string DssParameters { get; set; }
    }
}