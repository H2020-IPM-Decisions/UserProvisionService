using System.ComponentModel.DataAnnotations;

namespace H2020.IPMDecisions.UPR.Core.Dtos
{
    public class FieldCropPestDssForAdaptationDto : FieldCropPestDssForUpdateDto
    {
        [Required]
        public string Name { get; set; }
    }
}