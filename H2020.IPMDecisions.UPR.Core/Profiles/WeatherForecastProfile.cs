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

            // Entities to Dtos
            CreateMap<WeatherForecast, WeatherForecastDto>()
                .ReverseMap();
            CreateMap<WeatherForecast, WeatherForecastForUpdateDto>()
                .ReverseMap();

            // Dtos to entities
            CreateMap<WeatherForecastForCreationDto, WeatherForecast>();
            CreateMap<WeatherForecastForUpdateDto, WeatherForecast>();
            CreateMap<WeatherForecastForUpdateDto, WeatherForecastForCreationDto>()
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