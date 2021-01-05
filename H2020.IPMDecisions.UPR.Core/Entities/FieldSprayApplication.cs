using System;
using System.ComponentModel.DataAnnotations;

namespace H2020.IPMDecisions.UPR.Core.Entities
{
    public class FieldSprayApplication
    {
        [Key]
        public Guid Id { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public DateTime Time { get; set; }
        [Required]
        public Double Rate { get; set; }
        public Guid FieldCropPestdId { get; set; }
        public FieldCropPest FieldCropPest { get; set; }
    }
}