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
        public int ReScheduleCount { get; set; }
        public bool IsCustomDss { get; set; } = false;
        public string CustomName { get; set; } = "";
        public DateTime DssParametersLastUpdate { get; set; }

        public ICollection<FieldDssResult> FieldDssResults { get; set; }
        public List<FieldDssObservation> FieldDssObservations { get; set; }
    }
}