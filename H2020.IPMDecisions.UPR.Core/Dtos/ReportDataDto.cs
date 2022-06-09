using System.Collections.Generic;
using H2020.IPMDecisions.UPR.Core.Models;

namespace H2020.IPMDecisions.UPR.Core.Dtos
{
    public class ReportDataDto
    {
        public ReportDataDto()
        {
            Farm = new ReportDataFarm();
        }

        public string UserId { get; set; }
        public ReportDataFarm Farm { get; set; }
    }

    public class ReportDataFarm
    {
        public ReportDataFarm()
        {
            DssModels = new List<ReportDataDssModel>();
        }

        public CustomPointLocation Location { get; set; }
        public string Country { get; set; }
        public List<ReportDataDssModel> DssModels { get; set; }
    }

    public class ReportDataDssModel
    {
        public string ModelName { get; set; }
        public string ModelId { get; set; }
    }
}