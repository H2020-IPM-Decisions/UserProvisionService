using System;
using System.ComponentModel.DataAnnotations;
using H2020.IPMDecisions.UPR.Core.Models;

namespace H2020.IPMDecisions.UPR.Core.Dtos
{
    public class FieldObservationForCreationDto
    {
        public CustomPointLocation Location { get; set; }

        [Required(ErrorMessage = "Time is required")]
        [DataType(DataType.DateTime)]
        public DateTime Time { get; set; }

        [Required(ErrorMessage = "FieldCropPestId is required")]
        public Guid? FieldCropPestId { get; set; }
        public string Severity { get; set; }
    }
}