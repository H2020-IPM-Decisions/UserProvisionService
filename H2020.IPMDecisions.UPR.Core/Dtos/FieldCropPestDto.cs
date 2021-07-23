using System;
using System.Collections.Generic;

namespace H2020.IPMDecisions.UPR.Core.Dtos
{
    public class FieldCropPestDto
    {
        public Guid Id { get; set; }
        public CropPestDto CropPestDto { get; set; }
        public Guid FieldCropId { get; set; }
        public string PestEppoCode { get; set; }
        public IDictionary<string, string> PestLanguages { get; set; }
    }
}