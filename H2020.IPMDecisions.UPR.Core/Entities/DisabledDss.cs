using System;
using System.ComponentModel.DataAnnotations;

namespace H2020.IPMDecisions.UPR.Core.Entities
{
    public class DisabledDss
    {
        [Key]
        public Guid Id { get; set; }
        [Required]
        public string DssId { get; set; }
        [Required]
        public string DssVersion { get; set; }
        [Required]
        public string DssModelId { get; set; }
        [Required]
        public string DssModelVersion { get; set; }
    }
}