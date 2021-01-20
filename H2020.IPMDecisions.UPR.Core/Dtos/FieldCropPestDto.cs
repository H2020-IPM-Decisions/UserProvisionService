using System;

namespace H2020.IPMDecisions.UPR.Core.Dtos
{
    public class FieldCropPestDto
    {
        public Guid Id { get; set; }
        public CropPestDto CropPestDto { get; set; }
        public Guid FieldCropId { get; set; }
    }
}