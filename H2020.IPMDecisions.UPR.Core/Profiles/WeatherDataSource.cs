using H2020.IPMDecisions.UPR.Core.Dtos;
using H2020.IPMDecisions.UPR.Core.Entities;

namespace H2020.IPMDecisions.UPR.Core.Profiles
{
    public class WeatherDataSourceProfile : MainProfile
    {
        public WeatherDataSourceProfile()
        {
            CreateMap<WeatherDataSource, WeatherDataSourceDto>()
                .ReverseMap();
        }
    }
}