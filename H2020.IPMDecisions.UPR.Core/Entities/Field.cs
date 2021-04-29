using System;
using System.ComponentModel.DataAnnotations;

namespace H2020.IPMDecisions.UPR.Core.Entities
{
    public class Field
    {
        [Key]
        public Guid Id { get; set; }
        [Required]
        public string Name { get; set; }
        public DateTime? SowingDate { get; set; }
        public string Variety { get; set; }

        public Guid FarmId { get; set; }
        public Farm Farm { get; set; }

        public virtual FieldCrop FieldCrop { get; set; }
    }
}