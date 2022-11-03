using System.Collections.Generic;

namespace H2020.IPMDecisions.UPR.Core.Dtos
{
    public class EppoCodeTypeDto
    {
        public EppoCodeTypeDto()
        {
            this.EppoCodesDto = new List<EppoCodeDto>();

        }
        public string EppoCodeType { get; set; }
        public List<EppoCodeDto> EppoCodesDto { get; set; }
    }
}