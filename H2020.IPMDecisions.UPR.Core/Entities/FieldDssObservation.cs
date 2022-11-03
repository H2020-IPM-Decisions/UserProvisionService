using System;
using System.ComponentModel.DataAnnotations;
using NetTopologySuite.Geometries;

namespace H2020.IPMDecisions.UPR.Core.Entities
{
    public class FieldDssObservation
    {
        [Key]
        public Guid Id { get; set; }
        [Required]
        public Point Location { get; set; }
        [Required]
        public DateTime Time { get; set; }
        public Guid FieldCropPestDssId { get; set; }
        public FieldCropPestDss FieldCropPestDss { get; set; }
        public string DssObservation { get; set; }
    }
}