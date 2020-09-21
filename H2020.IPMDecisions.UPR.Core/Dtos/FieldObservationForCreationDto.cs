using System;
using System.ComponentModel.DataAnnotations;
using H2020.IPMDecisions.UPR.Core.Models;

namespace H2020.IPMDecisions.UPR.Core.Dtos
{
    public class FieldObservationForCreationDto 
    {
        [Required(ErrorMessage = "Location is required")]
        public CustomPointLocation Location { get; set; }

        [Required(ErrorMessage = "Time is required")]
        [DataType(DataType.DateTime)]
        public DateTime Time { get; set; }

        [Required(ErrorMessage = "Pest is required")]
        [MaxLength(6, ErrorMessage = "EPPO Codes max length is 6 characters")]
        public string PestEppoCode { get; set; }

        [Required(ErrorMessage = "Crop is required")]
        [MaxLength(6, ErrorMessage = "EPPO Codes max length is 6 characters")]
        public string CropEppoCode { get; set; }
    }
}