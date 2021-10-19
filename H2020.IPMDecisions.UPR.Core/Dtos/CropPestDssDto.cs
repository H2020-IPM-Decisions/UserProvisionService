using System;

namespace H2020.IPMDecisions.UPR.Core.Dtos
{
    public class CropPestDssDto
    {
        public Guid Id { get; set; }
        public string DssId { get; set; }
        public string DssName { get; set; }
        public string DssVersion { get; set; }
        public string DssModelId { get; set; }
        public string DssModelName { get; set; }
        public string DssModelVersion { get; set; }
    }
}