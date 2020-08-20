using H2020.IPMDecisions.UPR.Core.Dtos;
using H2020.IPMDecisions.UPR.Core.Entities;

namespace H2020.IPMDecisions.UPR.Core.Profiles
{
    public class CropPestProfile : MainProfile
    {
        public CropPestProfile()
        {
            // Entities to Dtos            

            // Dtos to Entities
            CreateMap<CropPestForCreationDto, CropPest>();
        }
    }
}