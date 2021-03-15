using H2020.IPMDecisions.UPR.Core.Dtos;
using H2020.IPMDecisions.UPR.Core.Entities;

namespace H2020.IPMDecisions.UPR.Core.Profiles
{
    public class UserWidgetProfile : MainProfile
    {
        public UserWidgetProfile()
        {
            // Entities to Dtos
            CreateMap<UserWidget, UserWidgetDto>()
                .ForMember(dest => dest.WidgetDescription, opt => opt.MapFrom(src => src.Widget.Description));
        }
    }
}