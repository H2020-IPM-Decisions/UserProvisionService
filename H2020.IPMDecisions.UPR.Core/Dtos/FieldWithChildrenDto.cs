using System;
using H2020.IPMDecisions.UPR.Core.Models;

namespace H2020.IPMDecisions.UPR.Core.Dtos
{
    public class FieldWithChildrenDto : FieldBaseDto
    {
        public ShapedDataWithLinks FieldObservationsDto { get; set; }
        public ShapedDataWithLinks FieldCropPestsDto { get; set; }
    }
}