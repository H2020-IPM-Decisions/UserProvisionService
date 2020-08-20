using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace H2020.IPMDecisions.UPR.Core.Dtos
{
    public abstract class FieldForManipulationDto
    {
        [MaxLength(80, ErrorMessage = "Name max length 80 characters")]
        public virtual string Name { get; set; }
        public virtual string Inf1 { get; set; }
        public virtual string Inf2 { get; set; }
        public ICollection<CropPestForCreationDto> CropPests { get; set; }
    }
}