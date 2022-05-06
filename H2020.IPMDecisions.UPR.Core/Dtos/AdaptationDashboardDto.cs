using Newtonsoft.Json.Linq;

namespace H2020.IPMDecisions.UPR.Core.Dtos
{
    public class AdaptationDashboardDto
    {
        public FieldDssResultDetailedDto DssOriginalResult { get; set; }
        public JObject DssOriginalParameters { get; set; }
    }
}