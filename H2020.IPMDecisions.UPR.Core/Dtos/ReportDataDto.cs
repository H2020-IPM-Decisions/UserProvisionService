using System.Collections.Generic;
using NetTopologySuite.Geometries;

namespace H2020.IPMDecisions.UPR.Core.Dtos
{
    public class ReportDataDto
    {
        public string UserId { get; set; }
        public List<ReportDataFarm> Farms { get; set; }
    }

    public class ReportDataFarm
    {
        public Point Location { get; set; }
        public string Country { get; set; }
        public List<ReportDataDssModel> DssModels { get; set; }
    }

    public class ReportDataDssModel
    {
        public string ModelName { get; set; }
        public string ModelId { get; set; }
    }
}