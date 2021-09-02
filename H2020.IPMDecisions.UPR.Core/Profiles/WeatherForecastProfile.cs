using H2020.IPMDecisions.UPR.Core.Dtos;
using H2020.IPMDecisions.UPR.Core.Entities;
using H2020.IPMDecisions.UPR.Core.Models;

namespace H2020.IPMDecisions.UPR.Core.Profiles
{
    public class WeatherForecastProfile : MainProfile
    {
        public WeatherForecastProfile()
        {
            CreateMap<WeatherForecast, WeatherForecast>()
                .ForMember(dest => dest.Id, opt => opt.Ignore());

            // Models to Entities
            CreateMap<WeatherDataSchema, WeatherForecast>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.WeatherId, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Url, opt => opt.MapFrom(src => src.EndPoint));

            // Entities to Dtos
            CreateMap<WeatherForecast, WeatherForecastDto>()
                .ReverseMap();
            CreateMap<WeatherForecast, WeatherServiceForUpdateDto>()
                .ReverseMap();

            // Dtos to entities
            CreateMap<WeatherServiceForCreationDto, WeatherForecast>();
            CreateMap<WeatherServiceForUpdateDto, WeatherForecast>();
            CreateMap<WeatherServiceForUpdateDto, WeatherServiceForCreationDto>()
                .ReverseMap();

            // Internal
            CreateMap<WeatherForecast, WeatherSchemaForHttp>()
                .AfterMap((src, dest) =>
                {
                    dest.IsForecast = true;
                });
        }
    }
}