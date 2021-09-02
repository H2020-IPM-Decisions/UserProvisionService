using H2020.IPMDecisions.UPR.Core.Dtos;
using H2020.IPMDecisions.UPR.Core.Entities;
using H2020.IPMDecisions.UPR.Core.Models;

namespace H2020.IPMDecisions.UPR.Core.Profiles
{
    public class WeatherStationProfile : MainProfile
    {
        public WeatherStationProfile()
        {
            CreateMap<WeatherHistorical, WeatherHistorical>()
                .ForMember(dest => dest.Id, opt => opt.Ignore());

            // Models to Entities
            CreateMap<WeatherDataSchema, WeatherHistorical>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.WeatherId, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Url, opt => opt.MapFrom(src => src.EndPoint));

            // Entities to Dtos
            CreateMap<WeatherHistorical, WeatherHistoricalDto>();
            CreateMap<WeatherHistorical, WeatherServiceForCreationDto>();
            CreateMap<WeatherHistorical, WeatherServiceForUpdateDto>();

            // Dtos to entities
            CreateMap<WeatherHistoricalDto, WeatherHistorical>();

            // Internal
            CreateMap<WeatherHistorical, WeatherSchemaForHttp>()
                .AfterMap((src, dest) =>
                {
                    dest.IsForecast = false;
                });
        }
    }
}