using H2020.IPMDecisions.UPR.Core.Dtos;
using H2020.IPMDecisions.UPR.Core.Entities;
using H2020.IPMDecisions.UPR.Core.Models;
using NetTopologySuite.Geometries;

namespace H2020.IPMDecisions.UPR.Core.Profiles
{
    public class GeometriesProfile : MainProfile
    {
        public GeometriesProfile()
        {
            // Object to Models
            CreateMap<Point, CustomPointLocation>()
                .ForMember(dest => dest.X,
                    opt => opt.MapFrom(src => src.X))
                .ForMember(dest => dest.Y,
                    opt => opt.MapFrom(src => src.Y))
                .ForMember(dest => dest.SRID,
                    opt => opt.MapFrom(src => src.SRID));
        }
    }
}