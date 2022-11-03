using System.ComponentModel.DataAnnotations;

namespace H2020.IPMDecisions.UPR.Core.Dtos
{
    public class CropPestForCreationDto
    {
        [Required(ErrorMessage = "Crop EPPO Code is required")]
        [MaxLength(6, ErrorMessage = "EPPO Codes max length is 6 characters")]
        public string CropEppoCode { get; set; }

        [Required(ErrorMessage = "Pest EPPO Code is required")]
        [MaxLength(6, ErrorMessage = "EPPO Codes max length is 6 characters")]
        public string PestEppoCode { get; set; }
    }
}