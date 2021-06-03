using H2020.IPMDecisions.UPR.Core.Dtos;
using H2020.IPMDecisions.UPR.Core.Entities;

namespace H2020.IPMDecisions.UPR.Core.Profiles
{
    public class EppoCodeProfile : MainProfile
    {
        public EppoCodeProfile()
        {
            // Entities to Dtos
            CreateMap<EppoCode, EppoCodeTypeDto>()
                .ForMember(dest => dest.EppoCodeType, opt => opt.MapFrom(src => src.Type));
        }
    }
}