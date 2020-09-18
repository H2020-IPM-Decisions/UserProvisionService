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
                    opt => opt.MapFrom(src => $"{src.FirstName} {src.LastName}"))
                .ForMember(dest => dest.FullAddress,
                    opt => opt.MapFrom(src => $"{src.UserAddress.Street} {src.UserAddress.City}, {src.UserAddress.Country}, {src.UserAddress.Postcode}"))
                .AfterMap((src, dest) => dest.FullName = dest.FullName.Trim())
                .AfterMap((src, dest) => dest.FullAddress = dest.FullAddress.Trim());

            CreateMap<UserProfile, UserProfileFullDto>()
                .ForMember(dest => dest.Street,
                        opt => opt.MapFrom(src => src.UserAddress.Street))
                .ForMember(dest => dest.City,
                        opt => opt.MapFrom(src => src.UserAddress.City))
                .ForMember(dest => dest.Country,
                        opt => opt.MapFrom(src => src.UserAddress.Country))
                .ForMember(dest => dest.Postcode,
                        opt => opt.MapFrom(src => src.UserAddress.Postcode));

            CreateMap<UserProfile, UserProfileForUpdateDto>();

            // Dtos to Entities
            CreateMap<UserProfileForCreationDto, UserProfile>()
                .ForPath(dest => dest.UserAddress.Street,
                    opt => opt.MapFrom(src => src.Street))
                .ForPath(dest => dest.UserAddress.City,
                    opt => opt.MapFrom(src => src.City))
                .ForPath(dest => dest.UserAddress.Country,
                    opt => opt.MapFrom(src => src.Country))
                .ForPath(dest => dest.UserAddress.Postcode,
                    opt => opt.MapFrom(src => src.Postcode));

            CreateMap<UserProfileForUpdateDto, UserProfile>()
                .ForPath(dest => dest.UserAddress.Street,
                    opt => opt.MapFrom(src => src.Street))
                .ForPath(dest => dest.UserAddress.City,
                    opt => opt.MapFrom(src => src.City))
                .ForPath(dest => dest.UserAddress.Country,
                    opt => opt.MapFrom(src => src.Country))
                .ForPath(dest => dest.UserAddress.Postcode,
                    opt => opt.MapFrom(src => src.Postcode));

            CreateMap<UserProfileInternalCallDto, UserProfile>()
                .ForPath(dest => dest.UserAddress.Street, opt => opt.MapFrom(s => ""));

            // Dtos to Dtos
            CreateMap<UserProfileForUpdateDto, UserProfileForCreationDto>();
        }
    }
}