using H2020.IPMDecisions.UPR.Core.Dtos;
using H2020.IPMDecisions.UPR.Core.Entities;

namespace H2020.IPMDecisions.UPR.Core.Profiles
{
    public class CropPestDssProfile : MainProfile
    {
        public CropPestDssProfile()
        {
            // Entities to Dtos
            CreateMap<CropPestDss, CropPestDssDto>();      

            // Dtos to Entities
            CreateMap<CropPestDssForCreationDto, CropPestDss>();
        }
    }
}