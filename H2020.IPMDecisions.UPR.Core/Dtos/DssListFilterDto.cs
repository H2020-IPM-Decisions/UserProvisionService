namespace H2020.IPMDecisions.UPR.Core.Dtos
{
    public class DssListFilterDto
    {
        public string CropCodes { get; set; } = "";
        public double LocationLatitude { get; set; } = 0;
        public double LocationLongitude { get; set; } = 0;
        public string ExecutionType { get; set; } = "";
        public string Country { get; set; } = "";
    }
}