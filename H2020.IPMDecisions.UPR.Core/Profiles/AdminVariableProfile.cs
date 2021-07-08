using H2020.IPMDecisions.UPR.Core.Dtos;
using H2020.IPMDecisions.UPR.Core.Entities;

namespace H2020.IPMDecisions.UPR.Core.Profiles
{
    public class AdminVariableProfile : MainProfile
    {
        public AdminVariableProfile()
        {
            // Entities to Dtos
            CreateMap<AdministrationVariable, AdminVariableDto>();

            // Dtos to Entities
            CreateMap<AdminVariableForManipulationDto, AdministrationVariable>()
                .ForMember(dest => dest.Description, opt => opt.Ignore())
                .ForMember(dest => dest.Id, opt => opt.Ignore());
        }
    }
}