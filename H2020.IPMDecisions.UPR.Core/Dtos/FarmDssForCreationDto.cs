using System;
using System.ComponentModel.DataAnnotations;

namespace H2020.IPMDecisions.UPR.Core.Dtos
{
    public class FarmDssForCreationDto
    {
        public Guid? FieldId { get; set; }
        public string FieldName { get; set; }
        [Required]
        public CropPestForCreationDto CropPest { get; set; }
        [Required]
        public string DssId { get; set; }
    }
}