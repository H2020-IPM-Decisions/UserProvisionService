using System.ComponentModel.DataAnnotations;

namespace H2020.IPMDecisions.UPR.Core.Dtos
{
    public class DssListFilterDto
    {
        [Required]
        public string CropCodes { get; set; }
        public double LocationLatitude { get; set; }
        public double LocationLongitude { get; set; }
        public string ExecutionType { get; set; } = "";
    }
}