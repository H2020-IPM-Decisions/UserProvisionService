using System;
using System.ComponentModel.DataAnnotations;

namespace H2020.IPMDecisions.UPR.Core.Entities
{
    public class CropPestDssResult
    {
        [Key]
        public Guid Id { get; set; }
        public Guid CropPestDssId { get; set; }
        public CropPestDss CropPestDss { get; set; }
        public DateTime CreationDate { get; set; }
    }
}