using System.Collections.Generic;

namespace H2020.IPMDecisions.UPR.Core.Models
{
    public class ReportData
    {
        public ReportData()
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