using H2020.IPMDecisions.UPR.Core.Dtos;
using H2020.IPMDecisions.UPR.Core.Entities;

namespace H2020.IPMDecisions.UPR.Core.Profiles
{
    public class UserProfileProfile : MainProfile
    {
        public UserProfileProfile()
        {
            // Entities to Dtos
            CreateMap<UserProfile, UserProfileDto>()
                .ForMember(dest => dest.FullName,
                opt => opt.MapFrom(src => $"{src.FirstName} {src.LastName}"));

            // Dtos to Entities
            CreateMap<UserProfileForCreationDto, UserProfile>();
        }
    }
}