using System;
using System.ComponentModel.DataAnnotations;

namespace H2020.IPMDecisions.UPR.Core.Dtos
{
    public class CropPestDssForCreationDto
    {
        [Required]
        public Guid FieldCropPestId { get; set; }
        [Required]
        public string DssId { get; set; }
        public string DssParameters { get; set; }
    }
}