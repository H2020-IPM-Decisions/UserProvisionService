using H2020.IPMDecisions.UPR.Core.Dtos;
using H2020.IPMDecisions.UPR.Core.Entities;

namespace H2020.IPMDecisions.UPR.Core.Profiles
{
    public class FieldProfile : MainProfile
    {
        public FieldProfile()
        {
            // Entities to Dtos
            CreateMap<Field, FieldDto>()
                .ForMember(dest => dest.FieldObservationsDto, opt => opt.MapFrom(src => src.FieldObservations));
            CreateMap<Field, FieldForUpdateDto>();

            // Dtos to Entities
            CreateMap<FieldForCreationDto, Field>();

            CreateMap<FieldForUpdateDto, Field>();

            // Dtos to Dtos
            CreateMap<FieldForUpdateDto, FieldForCreationDto>();
        }
    }
}