using H2020.IPMDecisions.UPR.Core.Dtos;

namespace H2020.IPMDecisions.UPR.Core.Profiles
{
    public class RiskMapsProfile : MainProfile
    {
        public RiskMapsProfile()
        {
            // Internal
            CreateMap<RiskMapBaseDto, RiskMapFullDetailDto>()
                .ReverseMap();
        }
    }
}