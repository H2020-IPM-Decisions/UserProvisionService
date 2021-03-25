using System.Text.Json;
using H2020.IPMDecisions.UPR.Core.Dtos;
using H2020.IPMDecisions.UPR.Core.Entities;

namespace H2020.IPMDecisions.UPR.Core.Profiles
{
    public class WeatherDataSourceProfile : MainProfile
    {
        public WeatherDataSourceProfile()
        {
            CreateMap<WeatherDataSource, WeatherDataSource>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.FieldWeatherDataSources, opt => opt.Ignore())
                .ForMember(dest => dest.FarmId, opt => opt.Ignore());

            CreateMap<WeatherDataSource, WeatherDataSourceDto>()
                .ForMember(dest => dest.Credentials, opt => opt.MapFrom((src, dest) =>
                {
                    var credentialsNoPassword = "";
                    if (string.IsNullOrEmpty(src.Credentials) || src.Credentials == "null") return credentialsNoPassword;

                    var credentialsAsObject = JsonSerializer.Deserialize<WeatherCredentials>(src.Credentials);
                    credentialsAsObject.Password = "*******";

                    credentialsNoPassword = JsonSerializer.Serialize(credentialsAsObject);
                    return credentialsNoPassword;
                }));

            CreateMap<WeatherDataSource, WeatherDataSourceForCreationDto>()
                .ForMember(dest => dest.Credentials, opt => opt.Ignore());

            CreateMap<WeatherDataSource, WeatherDataSourceForUpdateDto>()
                .ForMember(dest => dest.Credentials, opt => opt.MapFrom((src, dest) =>
                {
                    WeatherCredentials credentialsNoPassword = null;
                    if (string.IsNullOrEmpty(src.Credentials) || src.Credentials == "null") return credentialsNoPassword;

                    var credentialsAsObject = JsonSerializer.Deserialize<WeatherCredentials>(src.Credentials);
                    return credentialsAsObject;
                }));

            CreateMap<WeatherDataSourceDto, WeatherDataSource>();

            CreateMap<WeatherDataSourceForCreationDto, WeatherDataSource>()
                .ForMember(dest => dest.Credentials, opt => opt.MapFrom((src, dest) =>
                {
                    return JsonSerializer.Serialize(src.Credentials);
                }));

            CreateMap<WeatherForManipulationDto, WeatherDataSource>()
                .ForMember(dest => dest.Credentials, opt => opt.MapFrom((src, dest) =>
                {
                    return JsonSerializer.Serialize(src.Credentials);
                }));

            CreateMap<WeatherDataSourceForUpdateDto, WeatherDataSourceForCreationDto>()
                .ReverseMap();
        }
    }
}