using H2020.IPMDecisions.UPR.Core.Dtos;
using H2020.IPMDecisions.UPR.Core.Entities;

namespace H2020.IPMDecisions.UPR.Core.Profiles
{
    public class FieldSprayProfile : MainProfile
    {
        public FieldSprayProfile()
        {
            // Entities to Dtos
            CreateMap<FieldSprayApplication, FieldSprayApplicationDto>();
            CreateMap<FieldSprayApplication, FieldForUpdateDto>();

            // Dtos to Entities
            CreateMap<FieldSprayApplicationForCreationDto, FieldSprayApplication>();
        }
    }
}