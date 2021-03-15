using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using NetTopologySuite.Geometries;

namespace H2020.IPMDecisions.UPR.Core.Entities
{
    public class FieldObservation
    {
        [Key]
        public Guid Id { get; set; }
        public Point Location { get; set; }
        [Required]
        public DateTime Time { get; set; }
        public Guid FieldCropPestdId { get; set; }
        public FieldCropPest FieldCropPest { get; set; }
        public string Severity { get; set; }

        public ICollection<ObservationAlert> ObservationAlerts { get; set; }
    }
}