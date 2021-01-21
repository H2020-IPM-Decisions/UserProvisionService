using System;
using System.Collections.Generic;

namespace H2020.IPMDecisions.UPR.Core.Dtos
{
    public class FieldCropPestWithChildrenDto
    {
        public Guid Id { get; set; }
        public CropPestDto CropPestDto { get; set; }
        public ICollection<FieldCropPestDssDto> FieldCropPestDssDto { get; set; }
        public ICollection<FieldObservationDto> FieldObservationDto { get; set; }
        public ICollection<FieldSprayApplicationDto> FieldSprayApplicationDto { get; set; }
        public Guid FieldCropId { get; set; }
    }
}