using System.Text.Json;
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
            // .ForMember(dest => dest.FieldWeatherStations, opt => opt.Ignore())
            // .ForMember(dest => dest.FarmId, opt => opt.Ignore());

            CreateMap<WeatherHistorical, WeatherHistoricalDto>();

            CreateMap<WeatherHistorical, WeatherHistoricalForCreationDto>();

            CreateMap<WeatherHistorical, WeatherHistoricalForUpdateDto>();

            CreateMap<WeatherHistoricalDto, WeatherHistorical>();

            CreateMap<WeatherHistoricalForCreationDto, WeatherHistorical>();

            CreateMap<WeatherHistoricalForUpdateDto, WeatherHistoricalForCreationDto>()
                .ReverseMap();
            CreateMap<WeatherHistorical, WeatherHistoricalDto>()
                .ReverseMap();

            CreateMap<WeatherHistorical, WeatherSchemaForHttp>();
        }
    }
}