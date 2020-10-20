using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace H2020.IPMDecisions.UPR.Core.Entities
{
    public class Field
    {
        [Key]
        public Guid Id { get; set; }
        [Required]
        public string Name { get; set; }
        public string Inf1 { get; set; }
        public string Inf2 { get; set; }

        public Guid FarmId { get; set; }
        public Farm Farm { get; set; }
                
        public ICollection<FieldCropPest> FieldCropPests { get; set; }
    }
}