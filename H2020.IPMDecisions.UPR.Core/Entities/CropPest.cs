using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace H2020.IPMDecisions.UPR.Core.Entities
{
    public class CropPest
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        [MaxLength(6)]
        public string CropEppoCode { get; set; }
        
        [Required]
        [MaxLength(6)]
        public string PestEppoCode { get; set; }

        public ICollection<FieldCropPest> FieldCropPests { get; set; }
    }
}