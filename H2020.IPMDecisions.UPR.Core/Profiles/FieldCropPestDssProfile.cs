using H2020.IPMDecisions.UPR.Core.Dtos;
using H2020.IPMDecisions.UPR.Core.Entities;

namespace H2020.IPMDecisions.UPR.Core.Profiles
{
    public class FieldCropPestDssProfile : MainProfile
    {
        public FieldCropPestDssProfile()
        {
            // Entities to Dtos
            CreateMap<FieldCropPestDss, FieldCropPestDssDto>()
                .ForMember(dest => dest.FieldCropPestDto, opt => opt.MapFrom(src => src.FieldCropPest))
                .ForMember(dest => dest.CropPestDssDto, opt => opt.MapFrom(src => src.CropPestDss));
        }
    }
}