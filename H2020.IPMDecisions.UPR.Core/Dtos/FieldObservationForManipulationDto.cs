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

        [Required(ErrorMessage = "Pest is required")]
        [MaxLength(6, ErrorMessage = "EPPO Codes max length is 6 characters")]
        public virtual string PestEppoCode { get; set; }

        [Required(ErrorMessage = "Crop is required")]
        [MaxLength(6, ErrorMessage = "EPPO Codes max length is 6 characters")]
        public virtual string CropEppoCode { get; set; }
    }
}