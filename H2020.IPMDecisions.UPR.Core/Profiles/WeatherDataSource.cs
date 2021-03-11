using System.Text.Json;
using H2020.IPMDecisions.UPR.Core.Dtos;
using H2020.IPMDecisions.UPR.Core.Entities;

namespace H2020.IPMDecisions.UPR.Core.Profiles
{
    public class WeatherDataSourceProfile : MainProfile
    {
        public WeatherDataSourceProfile()
        {
            CreateMap<WeatherDataSource, WeatherDataSourceDto>()
                .ForMember(dest => dest.Credentials, opt => opt.MapFrom((src, dest) =>
                {
                    var credentialsNoPassword = "";
                    if (string.IsNullOrEmpty(src.Credentials)) return credentialsNoPassword;

                    var credentialsAsObject = JsonSerializer.Deserialize<WeatherCredentials>(src.Credentials);
                    credentialsAsObject.Password = "*******";

                    credentialsNoPassword = JsonSerializer.Serialize(credentialsAsObject);
                    return credentialsNoPassword;
                }));

            CreateMap<WeatherDataSource, WeatherDataSourceForCreationDto>()
                .ForMember(dest => dest.Credentials, opt => opt.Ignore());

            CreateMap<WeatherDataSource, WeatherDataSourceForUpdateDto>()
                .ForMember(dest => dest.Credentials, opt => opt.Ignore());

            CreateMap<WeatherDataSourceDto, WeatherDataSource>();

            CreateMap<WeatherDataSourceForCreationDto, WeatherDataSource>()
                .ForMember(dest => dest.Credentials, opt => opt.MapFrom((src, dest) =>
                {
                    var credentialsAsJson = "";
                    if (src.AuthenticationRequired == true)
                    {
                        credentialsAsJson = JsonSerializer.Serialize(src.Credentials);
                    }
                    return credentialsAsJson;
                }));

            CreateMap<WeatherDataSourceForUpdateDto, WeatherDataSourceForCreationDto>()
                .ReverseMap();
        }
    }
}