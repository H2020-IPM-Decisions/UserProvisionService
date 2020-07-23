using System;
using System.Collections.Generic;

namespace H2020.IPMDecisions.UPR.Core.Dtos
{
    public class FieldDto
    {
        public Guid Id { get; set; }
        public virtual string Name { get; set; }
        public virtual string Inf1 { get; set; }
        public virtual string Inf2 { get; set; }
        public Guid FarmId { get; set; }

        public ICollection<FieldObservationDto> FieldObservationsDto { get; set; }
    }
}