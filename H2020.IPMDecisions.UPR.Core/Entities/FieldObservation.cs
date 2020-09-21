using System;
using System.ComponentModel.DataAnnotations;
using NetTopologySuite.Geometries;

namespace H2020.IPMDecisions.UPR.Core.Entities
{
    public class FieldObservation
    {
        [Key]
        public Guid Id { get; set; }
        [Required]
        public Point Location { get; set; }
        [Required]
        public DateTime Time { get; set; }
        [Required]
        [MaxLength(6)]
        public string PestEppoCode { get; set; }
        [Required]
        [MaxLength(6)]
        public string CropEppoCode { get; set; }

        public Guid FieldId { get; set; }
        public Field Field { get; set; }
    }
}