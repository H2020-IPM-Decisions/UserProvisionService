using System;
using System.ComponentModel.DataAnnotations;

namespace H2020.IPMDecisions.UPR.Core.Entities
{
    public class FieldObservation
    {
        [Key]
        public Guid Id { get; set; }
        [Required]
        public string Inf1 { get; set; }
        public string Inf2 { get; set; }
        public string Inf3 { get; set; }

        public Guid FieldId { get; set; }
        public Field Field { get; set; }
    }
}