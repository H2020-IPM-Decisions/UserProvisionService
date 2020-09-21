using System;

namespace H2020.IPMDecisions.UPR.Core.Entities
{
    public class FieldCropPest
    {
        public Guid FieldId { get; set; }
        public Field Field { get; set; }
        public Guid CropPestId { get; set; }
        public CropPest CropPest { get; set; }
    }
}