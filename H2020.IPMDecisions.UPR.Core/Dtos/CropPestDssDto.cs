using System;

namespace H2020.IPMDecisions.UPR.Core.Dtos
{
    public class CropPestDssDto
    {
        public Guid Id { get; set; }
        public CropPestDto CropPestDto { get; set; }
        public string DssId { get; set; }
        public string DssName { get; set; }
    }
}