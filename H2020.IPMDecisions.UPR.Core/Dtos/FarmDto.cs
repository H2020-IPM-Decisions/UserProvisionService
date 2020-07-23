using System;
using System.Collections.Generic;
using H2020.IPMDecisions.UPR.Core.Models;

namespace H2020.IPMDecisions.UPR.Core.Dtos
{
    public class FarmDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Inf1 { get; set; }
        public string Inf2 { get; set; }
        public CustomPointLocation Location { get; set; }
        
        public ICollection<FieldDto> FieldsDto { get; set; }
    }
}