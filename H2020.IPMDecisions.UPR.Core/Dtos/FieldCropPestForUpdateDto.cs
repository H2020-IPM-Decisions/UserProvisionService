using System;
using System.ComponentModel.DataAnnotations;

namespace H2020.IPMDecisions.UPR.Core.Dtos
{
    public class FieldCropPestForUpdateDto
    {
        public Guid Id { get; set; }

        [Required(ErrorMessage = "Pest EPPO Code is required")]
        [MaxLength(6, ErrorMessage = "Pest Codes max length is 6 characters")]
        public string PestEppoCode { get; set; }
    }
}