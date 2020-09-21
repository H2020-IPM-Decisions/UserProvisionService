using H2020.IPMDecisions.UPR.Core.Dtos;
using H2020.IPMDecisions.UPR.Core.Entities;

namespace H2020.IPMDecisions.UPR.Core.Profiles
{
    public class FieldCropPestProfile : MainProfile
    {
        public FieldCropPestProfile()
        {
            // Entities to Dtos
            CreateMap<FieldCropPest, FieldCropPestDto>()
                .ForMember(dest => dest.CropPestDto, opt => opt.MapFrom(src => src.CropPest));
        }
    }
}