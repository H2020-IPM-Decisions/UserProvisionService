using System.Collections.Generic;

namespace H2020.IPMDecisions.UPR.Core.Dtos
{
    public class LinkDssDto
    {
        public string Name { get; set; }
        public string CropEppoCode { get; set; }
        public IDictionary<string, string> CropLanguages { get; set; }
        public string PestEppoCode { get; set; }
        public IDictionary<string, string> PestLanguages { get; set; }
        public string DssId { get; set; }
        public string DssName { get; set; }
        public string DssModelId { get; set; }
        public string DssModelName { get; set; }
        public string DssEndPoint { get; set; }
        public IEnumerable<string> ValidatedSpatialCountries { get; set; }
    }
}