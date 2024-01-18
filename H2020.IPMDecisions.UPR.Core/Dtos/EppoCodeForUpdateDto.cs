using System.ComponentModel.DataAnnotations;

namespace H2020.IPMDecisions.UPR.Core.Dtos
{
    public class EppoCodeForUpdateDto
    {
        [Required]
        public string EppoCodes { get; set; }
    }
}