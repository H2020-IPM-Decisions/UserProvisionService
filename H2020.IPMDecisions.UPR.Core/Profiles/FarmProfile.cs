using H2020.IPMDecisions.UPR.Core.Dtos;
using H2020.IPMDecisions.UPR.Core.Entities;

namespace H2020.IPMDecisions.UPR.Core.Profiles
{
    public class FarmProfile : MainProfile
    {
        public FarmProfile()
        {
            // Entities to Dtos
            CreateMap<Farm, FarmDto>();

            // Dtos to Entities
            CreateMap<FarmForCreationDto, Farm>();

            CreateMap<FarmForUpdateDto, Farm>();

            // Dtos to Dtos
            CreateMap<FarmForUpdateDto, FarmForCreationDto>();
        }
    }
}