using System;

namespace H2020.IPMDecisions.UPR.Core.Dtos
{
    public class FarmDssDto : FarmBaseDto
    {
        public Guid? FieldId { get; set; }
        public string FieldName { get; set; }
        public FieldCropPestDssDto FieldCropPestDssDto { get; set; }
        public string DssId { get; set; }
    }
}