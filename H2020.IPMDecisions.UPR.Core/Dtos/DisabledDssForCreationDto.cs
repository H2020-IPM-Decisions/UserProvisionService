using System;

namespace H2020.IPMDecisions.UPR.Core.Dtos
{
    public class DisabledDssForCreationDto
    {
        public string DssId { get; set; }
        public string DssVersion { get; set; }
        public string DssModelId { get; set; }
        public string DssModelVersion { get; set; }
    }
}