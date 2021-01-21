using H2020.IPMDecisions.UPR.Core.Dtos;
using H2020.IPMDecisions.UPR.Core.Entities;

namespace H2020.IPMDecisions.UPR.Core.Profiles
{
    public class FieldCropPestProfile : MainProfile
    {
        public FieldCropPestProfile()
        {
            // Entities to Dtos
            CreateMap<FieldCropPest, FieldCropPestWithChildrenDto>()
                .ForMember(dest => dest.CropPestDto, opt => opt.MapFrom(src => src.CropPest))
                .ForMember(dest => dest.FieldCropId, opt => opt.MapFrom(src => src.FieldCropId));

            CreateMap<FieldCropPest, FieldCropPestDto>()
                .ForMember(dest => dest.CropPestDto, opt => opt.MapFrom(src => src.CropPest))
                .ForMember(dest => dest.FieldCropId, opt => opt.MapFrom(src => src.FieldCropId));

            CreateMap<FieldCrop, FieldCropDto>()
                .ForMember(dest => dest.FieldCropPestWithChildrenDto, opt => opt.Ignore());

            CreateMap<FieldCrop, FieldCropWithChildrenDto>()
                .ForMember(dest => dest.FieldCropPestDto, opt => opt.MapFrom(src => src.FieldCropPests));
        }
    }
}