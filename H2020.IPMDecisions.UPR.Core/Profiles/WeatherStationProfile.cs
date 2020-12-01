using H2020.IPMDecisions.UPR.Core.Dtos;
using H2020.IPMDecisions.UPR.Core.Entities;

namespace H2020.IPMDecisions.UPR.Core.Profiles
{
    public class WeatherStationProfile : MainProfile
    {
        public WeatherStationProfile()
        {
            CreateMap<WeatherStation, WeatherStationDto>()
                .ReverseMap();
        }
    }
}