using H2020.IPMDecisions.UPR.Core.Dtos;
using H2020.IPMDecisions.UPR.Core.Entities;
using H2020.IPMDecisions.UPR.Core.Models;

namespace H2020.IPMDecisions.UPR.Core.Profiles
{
    public class WeatherHistoricalProfile : MainProfile
    {
        public WeatherHistoricalProfile()
        {
            CreateMap<WeatherHistorical, WeatherHistorical>()
                .ForMember(dest => dest.Id, opt => opt.Ignore());

            // Models to Entities
            CreateMap<WeatherDataSchema, WeatherHistorical>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.WeatherId, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Url, opt => opt.MapFrom(src => src.EndPoint))
                .AfterMap((src, dest, context) =>
                {
                    dest.Url = dest.Url.Replace("{WEATHER_API_URL}", context.Items["host"].ToString());
                });

            // Entities to Dtos
            CreateMap<WeatherHistorical, WeatherHistoricalDto>();
            CreateMap<WeatherHistorical, WeatherServiceForCreationDto>();
            CreateMap<WeatherHistorical, WeatherServiceForUpdateDto>();

            // Dtos to entities
            CreateMap<WeatherHistoricalDto, WeatherHistorical>();
        }
    }
}