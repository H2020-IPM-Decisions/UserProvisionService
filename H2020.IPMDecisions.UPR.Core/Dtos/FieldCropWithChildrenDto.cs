using System;
using System.Collections.Generic;

namespace H2020.IPMDecisions.UPR.Core.Dtos
{
    public class FieldCropWithChildrenDto
    {
        public Guid Id { get; set; }
        public string CropEppoCode { get; set; }
        public IDictionary<string, string> CropLanguages { get; set; }
        public IEnumerable<FieldCropPestDto> FieldCropPestDto { get; set; }
    }
}