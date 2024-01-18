using System.ComponentModel.DataAnnotations;

namespace H2020.IPMDecisions.UPR.Core.Dtos
{
    public class EppoCodeForCreationDto
    {
        [Required]
        public string EppoCodeType { get; set; }
        [Required]
        public string EppoCodes { get; set; }
    }
}