namespace H2020.IPMDecisions.UPR.Core.Dtos
{
    public class RiskMapFullDetailDto : RiskMapBaseDto
    {
        public string PlatformValidated { get; set; }
        public string ProviderCountry { get; set; }
        public string ProviderAddress { get; set; }
        public string ProviderPostalCode { get; set; }
        public string ProviderCity { get; set; }
        public string ProviderEmail { get; set; }
        public string ProviderUrl { get; set; }
    }
}