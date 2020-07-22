using H2020.IPMDecisions.UPR.Core.Dtos;
using H2020.IPMDecisions.UPR.Core.Entities;
using NetTopologySuite.Geometries;

namespace H2020.IPMDecisions.UPR.Core.Profiles
{
    public class FieldObservationProfile : MainProfile
    {
        public FieldObservationProfile()
        {
            // Entities to Dtos
            CreateMap<FieldObservation, FieldObservationDto>();
            CreateMap<FieldObservation, FieldForUpdateDto>();

            // Dtos to Entities
            CreateMap<FieldObservationForCreationDto, FieldObservation>()
                .AfterMap((src, dest) =>
                {
                    dest.Location = new Point(src.Location.X, src.Location.Y) { SRID = src.Location.SRID };
                });
        }
    }
}