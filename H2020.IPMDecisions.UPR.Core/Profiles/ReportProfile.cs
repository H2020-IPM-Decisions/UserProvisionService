using System.Linq;
using H2020.IPMDecisions.UPR.Core.Dtos;
using H2020.IPMDecisions.UPR.Core.Entities;

namespace H2020.IPMDecisions.UPR.Core.Profiles
{
    public class ReportProfile : MainProfile
    {
        public ReportProfile()
        {
            // Entities to Dtos
            CreateMap<UserFarm, ReportDataDto>()
                .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.UserId));
        }
    }
}