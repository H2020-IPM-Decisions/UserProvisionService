using H2020.IPMDecisions.UPR.Core.Dtos;
using H2020.IPMDecisions.UPR.Core.Entities;
using H2020.IPMDecisions.UPR.Core.Models;

namespace H2020.IPMDecisions.UPR.Core.Profiles
{
    public class DisabledDssProfile : MainProfile
    {
        public DisabledDssProfile()
        {
            // Entities to Dtos
            CreateMap<DisabledDss, DisabledDssDto>()
                .ReverseMap();

            CreateMap<DisabledDssForCreationDto, DisabledDss>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ReverseMap();

            CreateMap<DssInformationJoined, DisabledDssDto>()
                .ForMember(dest => dest.DssId, opt => opt.MapFrom(src => src.DssInformation.Id))
                .ForMember(dest => dest.DssName, opt => opt.MapFrom(src => src.DssInformation.Name))
                .ForMember(dest => dest.DssModelId, opt => opt.MapFrom(src => src.DssModelInformation.Id))
                .ForMember(dest => dest.DssModelName, opt => opt.MapFrom(src => src.DssModelInformation.Name))
                .ForMember(dest => dest.DssVersion, opt => opt.MapFrom(src => src.DssInformation.Version))
                .ForMember(dest => dest.DssModelVersion, opt => opt.MapFrom(src => src.DssModelInformation.Version))
                .ForMember(dest => dest.IsDisabled, opt => opt.MapFrom(src => false));
        }
    }
}