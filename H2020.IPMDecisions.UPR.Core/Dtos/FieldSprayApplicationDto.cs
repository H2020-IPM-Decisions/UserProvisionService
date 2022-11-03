using System;

namespace H2020.IPMDecisions.UPR.Core.Dtos
{
    public class FieldSprayApplicationDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public DateTime Time { get; set; }
        public Double Rate { get; set; }
    }
}