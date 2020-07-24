using System.Collections.Generic;

namespace H2020.IPMDecisions.UPR.Core.Dtos
{
    public class FarmDto : FarmBaseDto
    {
        public ICollection<FieldDto> FieldsDto { get; set; }
    }
}