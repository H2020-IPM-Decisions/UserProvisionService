using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace H2020.IPMDecisions.UPR.Core.Entities
{
    public class FieldCrop
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        [MaxLength(6)]
        public string CropEppoCode { get; set; }
        public Guid FieldId { get; set; }
        public Field Field { get; set; }

        public ICollection<FieldCropPest> FieldCropPests { get; set; }
    }
}