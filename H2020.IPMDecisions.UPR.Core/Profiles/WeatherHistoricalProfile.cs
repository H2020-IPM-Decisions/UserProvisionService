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

            // Entities to Dtos
            CreateMap<WeatherHistorical, WeatherHistoricalDto>();
            CreateMap<WeatherHistorical, WeatherHistoricalForCreationDto>();
            CreateMap<WeatherHistorical, WeatherHistoricalForUpdateDto>();

            // Dtos to entities
            CreateMap<WeatherHistoricalDto, WeatherHistorical>();
            CreateMap<WeatherHistoricalForCreationDto, WeatherHistorical>();
            CreateMap<WeatherHistoricalForUpdateDto, WeatherHistoricalForCreationDto>()
                .ReverseMap();

            // Internal
            CreateMap<WeatherHistorical, WeatherSchemaForHttp>();
        }
    }
}