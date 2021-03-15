using System;
using H2020.IPMDecisions.UPR.Core.Dtos;
using H2020.IPMDecisions.UPR.Core.Entities;

namespace H2020.IPMDecisions.UPR.Core.Profiles
{
    public class DssResultProfile : MainProfile
    {
        public DssResultProfile()
        {
            // Entities to Dtos
            CreateMap<FieldDssResult, FieldDssResultDto>();

            // Dtos to Entities
            CreateMap<FieldDssResultForCreationDto, FieldDssResult>()
                .ForMember(dest => dest.CreationDate, opt => opt.MapFrom(src => DateTime.Now));
        }
    }
}