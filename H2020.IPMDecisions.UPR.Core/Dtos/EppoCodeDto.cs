using System.Collections.Generic;

namespace H2020.IPMDecisions.UPR.Core.Dtos
{
    public class EppoCodeDto
    {
        public string EppoCode { get; set; }
        public IDictionary<string, string> Languages { get; set; }
    }
}