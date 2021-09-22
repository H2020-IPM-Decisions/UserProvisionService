using H2020.IPMDecisions.UPR.Core.Dtos;
using H2020.IPMDecisions.UPR.Core.Entities;

namespace H2020.IPMDecisions.UPR.Core.Profiles
{
    public class CropPestDssProfile : MainProfile
    {
        public CropPestDssProfile()
        {
            // Entities to Dtos
            CreateMap<CropPestDss, CropPestDssDto>();

            // Dtos to Entities
            CreateMap<CropPestDssForCreationDto, CropPestDss>()
                .ForMember(dest => dest.DssExecutionType, opt => opt.MapFrom(src => src.DssExecutionType.ToLower()));

            CreateMap<FarmDssForCreationDto, CropPestDss>()
                .ForMember(dest => dest.CropPest, opt => opt.Ignore())
                .ForMember(dest => dest.FieldCropPestDsses, opt => opt.Ignore())
                .ForMember(dest => dest.DssExecutionType, opt => opt.MapFrom(src => src.DssExecutionType.ToLower()));

            CreateMap<FarmDssForCreationDto, CropPestForCreationDto>()
                .ForMember(dest => dest.CropEppoCode, opt => opt.MapFrom(src => src.CropEppoCode.ToUpper()))
                .ForMember(dest => dest.PestEppoCode, opt => opt.MapFrom(src => src.PestEppoCode.ToUpper()));
        }
    }
}