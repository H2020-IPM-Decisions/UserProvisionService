using System;

namespace H2020.IPMDecisions.UPR.Core.Dtos
{
    public class FieldCropPestWithChildrenDto
    {
        public Guid Id { get; set; }
        public CropPestDto CropPestDto { get; set; }
        public Guid FieldCropId { get; set; }
        public string PestEppoCode { get; set; }
    }
}