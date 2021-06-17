using System.ComponentModel.DataAnnotations;

namespace H2020.IPMDecisions.UPR.Core.Entities
{
    public class EppoCode
    {
        [Key]
        public string Type { get; set; }
        [Required]
        public string Data { get; set; }
    }
}