using System;
using System.ComponentModel.DataAnnotations;
using H2020.IPMDecisions.UPR.Core.Models;

namespace H2020.IPMDecisions.UPR.Core.Dtos
{
    public abstract class FieldObservationForManipulationDto
    {
        [Required(ErrorMessage = "Location is required")]
        public virtual CustomPointLocation Location { get; set; }

        [Required(ErrorMessage = "Time is required")]
        [DataType(DataType.DateTime)]
        public virtual DateTime Time { get; set; }

        [Required(ErrorMessage = "FieldCropPestId is required")]
        public Guid? FieldCropPestId { get; set; }
    }
}