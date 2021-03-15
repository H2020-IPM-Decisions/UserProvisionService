namespace H2020.IPMDecisions.UPR.Core.Models
{
    public class DssExecutionInformation
    {
        public string Type { get; set; }
        public string EndPoint { get; set; }
        public bool UsesWeatherData { get; set; } = false;
    }
}