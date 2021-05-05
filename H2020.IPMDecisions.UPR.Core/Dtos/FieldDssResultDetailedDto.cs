using System.Collections.Generic;

namespace H2020.IPMDecisions.UPR.Core.Dtos
{
    public class FieldDssResultDetailedDto : FieldDssResultBaseDto
    {
        public List<ResultParameters> ResultParameters { get; set; }
    }

    public class ResultParameters
    {
        public string Code { get; set; }
        public string Description { get; set; }
        public List<int> Data { get; set; }
    }
}