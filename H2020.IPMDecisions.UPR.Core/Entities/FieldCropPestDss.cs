using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace H2020.IPMDecisions.UPR.Core.Entities
{
    public class FieldCropPestDss
    {
        [Key]
        public Guid Id { get; set; }
        public Guid FieldCropPestId { get; set; }
        public FieldCropPest FieldCropPest { get; set; }
        public Guid CropPestDssId { get; set; }
        public CropPestDss CropPestDss { get; set; }
        public string DssParameters { get; set; }
        public bool ObservationRequired { get; set; }
        public string LastJobId { get; set; }

        public ICollection<FieldDssResult> FieldDssResults { get; set; }
        public List<FieldDssObservation> FieldDssObservations { get; set; }
    }
}