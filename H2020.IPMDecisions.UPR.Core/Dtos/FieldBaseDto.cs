using System;

namespace H2020.IPMDecisions.UPR.Core.Dtos
{
    public abstract class FieldBaseDto
    {
        public Guid Id { get; set; }
        public virtual string Name { get; set; }
        public virtual DateTime? SowingDate { get; set; }
        public virtual string Variety { get; set; }
        public Guid FarmId { get; set; }
    }
}