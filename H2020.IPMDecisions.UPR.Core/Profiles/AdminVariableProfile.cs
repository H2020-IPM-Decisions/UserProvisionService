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
        }
    }
}