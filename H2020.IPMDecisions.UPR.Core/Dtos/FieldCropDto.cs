using System;
using System.Collections.Generic;
using H2020.IPMDecisions.UPR.Core.Models;

namespace H2020.IPMDecisions.UPR.Core.Dtos
{
    public class FieldCropDto
    {
        public Guid Id { get; set; }
        public string CropEppoCode { get; set; }
        public IDictionary<string, string> CropLanguages { get; set; }
        public ShapedDataWithLinks FieldCropPestDto { get; set; }
    }
}