using H2020.IPMDecisions.UPR.Core.Dtos;
using H2020.IPMDecisions.UPR.Core.Models;

namespace H2020.IPMDecisions.UPR.Core.Profiles
{
    public class DssOutputProfile : MainProfile
    {
        public DssOutputProfile()
        {
            CreateMap<OutputChartGroup, ChartGroup>();
        }
    }
}