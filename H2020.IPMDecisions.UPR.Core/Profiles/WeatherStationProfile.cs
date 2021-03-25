using System.Text.Json;
using H2020.IPMDecisions.UPR.Core.Dtos;
using H2020.IPMDecisions.UPR.Core.Entities;

namespace H2020.IPMDecisions.UPR.Core.Profiles
{
    public class WeatherStationProfile : MainProfile
    {
        public WeatherStationProfile()
        {
            CreateMap<WeatherStation, WeatherStation>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.FieldWeatherStations, opt => opt.Ignore())
                .ForMember(dest => dest.FarmId, opt => opt.Ignore());

            CreateMap<WeatherStation, WeatherStationDto>()
                .ForMember(dest => dest.Credentials, opt => opt.MapFrom((src, dest) =>
                {
                    var credentialsNoPassword = "";
                    if (string.IsNullOrEmpty(src.Credentials) || src.Credentials == "null") return credentialsNoPassword;

                    var credentialsAsObject = JsonSerializer.Deserialize<WeatherCredentials>(src.Credentials);
                    credentialsAsObject.Password = "*******";

                    credentialsNoPassword = JsonSerializer.Serialize(credentialsAsObject);
                    return credentialsNoPassword;
                }));

            CreateMap<WeatherStation, WeatherStationForCreationDto>()
                .ForMember(dest => dest.Credentials, opt => opt.Ignore());

            CreateMap<WeatherStation, WeatherStationForUpdateDto>()
                .ForMember(dest => dest.Credentials, opt => opt.MapFrom((src, dest) =>
                {
                    WeatherCredentials credentialsNoPassword = null;
                    if (string.IsNullOrEmpty(src.Credentials) || src.Credentials == "null") return credentialsNoPassword;

                    var credentialsAsObject = JsonSerializer.Deserialize<WeatherCredentials>(src.Credentials);
                    return credentialsAsObject;
                }));

            CreateMap<WeatherStationDto, WeatherStation>();

            CreateMap<WeatherStationForCreationDto, WeatherStation>()
                .ForMember(dest => dest.Credentials, opt => opt.MapFrom((src, dest) =>
                {
                    return JsonSerializer.Serialize(src.Credentials);
                }));

            CreateMap<WeatherForManipulationDto, WeatherStation>()
                .ForMember(dest => dest.Credentials, opt => opt.MapFrom((src, dest) =>
                {
                    return JsonSerializer.Serialize(src.Credentials);
                }));

            CreateMap<WeatherStationForUpdateDto, WeatherStationForCreationDto>()
                .ReverseMap();

            CreateMap<WeatherStation, WeatherStationDto>()
                .ReverseMap();
        }
    }
}