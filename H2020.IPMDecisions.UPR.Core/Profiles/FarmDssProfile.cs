using System.Linq;
using H2020.IPMDecisions.UPR.Core.Dtos;
using H2020.IPMDecisions.UPR.Core.Entities;

namespace H2020.IPMDecisions.UPR.Core.Profiles
{
    public class FarmDssProfile : MainProfile
    {
        public FarmDssProfile()
        {
            // Entities to Dtos
            CreateMap<Farm, FarmDssDto>()
            .AfterMap((src, dest, context) =>
                {
                }); ;
        }
    }
}