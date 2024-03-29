using System.ComponentModel.DataAnnotations;
using H2020.IPMDecisions.UPR.Core.Models;

namespace H2020.IPMDecisions.UPR.Core.Dtos
{
    public abstract class FarmForManipulationDto
    {
        [MaxLength(80, ErrorMessage = "Name max length 80 characters")]
        public virtual string Name { get; set; }
        public virtual CustomPointLocation Location { get; set; }
    }
}