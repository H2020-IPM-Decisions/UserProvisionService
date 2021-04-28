using System;
using System.ComponentModel.DataAnnotations;

namespace H2020.IPMDecisions.UPR.Core.Dtos
{
    public abstract class FieldForManipulationDto
    {
        [MaxLength(80, ErrorMessage = "Name max length 80 characters")]
        public virtual string Name { get; set; }
        public virtual DateTime? SowingDate { get; set; }
        public virtual string Variety { get; set; }

    }
}