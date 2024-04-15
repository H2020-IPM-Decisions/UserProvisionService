using System.Collections.Generic;

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
        public MapConfiguration MapConfiguration { get; set; }
    }

    public class MapConfiguration
    {
        public string Name { get; set; }
        public string Title { get; set; }
        public string Abstract { get; set; }
        public string Projection { get; set; }
        public List<LayerConfiguration> LayersConfiguration { get; set; } = new List<LayerConfiguration>();
    }

    public class LayerConfiguration
    {
        public string Name { get; set; }
        public string Title { get; set; }
        public List<string> Dates { get; set; }
        public string LegendURL { get; set; }
        public dynamic LegendMetadata { get; set; }
    }
}