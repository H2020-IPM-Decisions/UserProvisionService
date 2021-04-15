using System.Collections.Generic;
using System.Linq;
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
            CreateMap<Farm, FarmDto>()
                .ForMember(dest => dest.WeatherForecastDto, opt => opt.MapFrom(src => src.WeatherForecast))
                .ForMember(dest => dest.WeatherHistoricalDto, opt => opt.MapFrom(src => src.WeatherHistorical));

            CreateMap<Farm, FarmWithChildrenDto>()
               .ForMember(dest => dest.FieldsDto, opt => opt.Ignore());

            CreateMap<Farm, FarmForUpdateDto>()
                .ForMember(dest => dest.WeatherForecastDto, opt => opt.MapFrom(src => src.WeatherForecast))
                .ForMember(dest => dest.WeatherHistoricalDto, opt => opt.MapFrom(src => src.WeatherHistorical));

            // Dtos to Entities
            CreateMap<FarmForCreationDto, Farm>()
                .ForMember(dest => dest.WeatherForecast, opt => opt.Ignore())
                .ForMember(dest => dest.WeatherHistorical, opt => opt.Ignore())
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