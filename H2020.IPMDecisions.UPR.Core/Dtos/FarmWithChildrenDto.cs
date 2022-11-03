using H2020.IPMDecisions.UPR.Core.Models;
using System;

namespace H2020.IPMDecisions.UPR.Core.Dtos
{
    public class FarmWithChildrenDto : FarmBaseDto
    {
        public ShapedDataWithLinks FieldsDto { get; set; }
    }
}