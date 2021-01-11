using System;
using System.ComponentModel.DataAnnotations;

namespace H2020.IPMDecisions.UPR.Core.Dtos
{
    public class FieldSprayApplicationForCreationDto
    {
        [Required(ErrorMessage = "FieldCropPestId is required")]
        public Guid? FieldCropPestId { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public DateTime Time { get; set; }
        [Required]
        public Double? Rate { get; set; }

    }
}