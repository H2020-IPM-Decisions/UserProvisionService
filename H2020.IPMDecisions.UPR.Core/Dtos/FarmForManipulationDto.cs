using System.ComponentModel.DataAnnotations;
using NetTopologySuite.Geometries;

namespace H2020.IPMDecisions.UPR.Core.Dtos
{
    public abstract class FarmForManipulationDto
    {
        [MaxLength(80, ErrorMessage = "Name max length 100 characters")]
        public virtual string Name { get; set; }
        public virtual string Inf1 { get; set; }
        public virtual string Inf2 { get; set; }
        public virtual Point Location { get; set; }
    }
}