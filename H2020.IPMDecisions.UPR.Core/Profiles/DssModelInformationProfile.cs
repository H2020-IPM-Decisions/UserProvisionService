using System.Linq;
using H2020.IPMDecisions.UPR.Core.Dtos;
using H2020.IPMDecisions.UPR.Core.Models;

namespace H2020.IPMDecisions.UPR.Core.Profiles
{
    public class DssModelInformationProfile : MainProfile
    {
        public DssModelInformationProfile()
        {
            // Models to Dtos
            CreateMap<DssInformationJoined, LinkDssDto>()
                .ForMember(dest => dest.DssId, opt => opt.MapFrom(src => src.DssInformation.Id))
                .ForMember(dest => dest.DssName, opt => opt.MapFrom(src => src.DssInformation.Name))
                .ForMember(dest => dest.DssModelId, opt => opt.MapFrom(src => src.DssModelInformation.Id))
                .ForMember(dest => dest.DssModelName, opt => opt.MapFrom(src => src.DssModelInformation.Name))
                .ForMember(dest => dest.DssEndPoint, opt => opt.MapFrom(src => src.DssModelInformation.Execution.EndPoint))
                .ForMember(dest => dest.CropEppoCode, opt => opt.MapFrom(src => src.DssModelInformation.Crops.FirstOrDefault()))
                .ForMember(dest => dest.PestEppoCode, opt => opt.MapFrom(src => src.DssModelInformation.Pests.FirstOrDefault()))
                .ForMember(dest => dest.ValidatedSpatialCountries, opt => opt.MapFrom(src => src.DssModelInformation.ValidSpatial.Countries));

            CreateMap<DssModelAuthors, DssModelAuthorsDto>();
        }
    }
}