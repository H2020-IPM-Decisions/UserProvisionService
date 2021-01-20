using H2020.IPMDecisions.UPR.Core.Models;

namespace H2020.IPMDecisions.UPR.Core.Dtos
{
    public class FieldWithChildrenDto : FieldBaseDto
    {
        public ShapedDataWithLinks FieldObservationsDto { get; set; }
        public FieldCropDto FieldCropDto { get; set; }
        public ShapedDataWithLinks FieldSpraysDto { get; set; }
    }
}