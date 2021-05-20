using System.Collections.Generic;

namespace H2020.IPMDecisions.UPR.Core.Models
{
    // This class matches the schema definition of /api/dss/rest/schema/modeloutput
    public class DssModelOutputInformation
    {
        public string TimeStart { get; set; }
        public string TimeEnd { get; set; }
        public string Interval { get; set; }
        public List<string> ResultParameters { get; set; }
        public string Message { get; set; }
        public int? MessageType { get; set; }
        public List<LocationResultDssOutput> LocationResult { get; set; }
    }

    public class LocationResultDssOutput
    {
        public double? Longitude { get; set; }
        public double? Latitude { get; set; }
        public int Width { get; set; }
        public int Length { get; set; }
        public List<int> WarningStatus { get; set; }
        public List<List<double>> Data { get; set; }
    }
}