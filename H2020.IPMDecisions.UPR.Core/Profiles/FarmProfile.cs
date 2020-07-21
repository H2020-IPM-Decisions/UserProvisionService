using H2020.IPMDecisions.UPR.Core.Dtos;
using H2020.IPMDecisions.UPR.Core.Entities;
using NetTopologySuite.Geometries;

namespace H2020.IPMDecisions.UPR.Core.Profiles
{
    public class FarmProfile : MainProfile
    {
        public FarmProfile()
        {
            // Entities to Dtos
            CreateMap<Farm, FarmDto>();
            CreateMap<Farm, FarmForUpdateDto>();

            // Dtos to Entities
            CreateMap<FarmForCreationDto, Farm>()
                .AfterMap((src, dest) =>
                {
                    dest.Location = new Point(src.Location.X, src.Location.Y) { SRID = src.Location.SRID };
                });

            CreateMap<FarmForUpdateDto, Farm>()
                .AfterMap((src, dest) =>
                {
                    dest.Location = new Point(src.Location.X, src.Location.Y) { SRID = src.Location.SRID };
                });

            // Dtos to Dtos
            CreateMap<FarmForUpdateDto, FarmForCreationDto>();
        }
    }
}