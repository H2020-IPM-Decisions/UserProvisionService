using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace H2020.IPMDecisions.UPR.Core.Entities
{
    public class CropPestDss
    {
        [Key]
        public Guid Id { get; set; }
        public Guid CropPestId { get; set; }
        public CropPest CropPest { get; set; }
        public string DssId { get; set; }
        [Required]
        public string DssName { get; set; }
    }
}