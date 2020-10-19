using System;

namespace H2020.IPMDecisions.UPR.Core.Dtos
{
    public class FieldCropPestDssDto
    {
        public Guid Id { get; set; }
        public Guid FieldCropPestId { get; set; }
        public FieldCropPestDto FieldCropPestDto { get; set; }
        public Guid CropPestDssId { get; set; }
        public CropPestDssDto CropPestDssDto { get; set; }
    }
}