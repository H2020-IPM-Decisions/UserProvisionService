using System.Text.Json;
using H2020.IPMDecisions.UPR.Core.Dtos;
using H2020.IPMDecisions.UPR.Core.Entities;
using H2020.IPMDecisions.UPR.Core.Models;

namespace H2020.IPMDecisions.UPR.Core.Profiles
{
    public class WeatherDataSourceProfile : MainProfile
    {
        public WeatherDataSourceProfile()
        {
            CreateMap<WeatherForecast, WeatherForecast>()
                .ForMember(dest => dest.Id, opt => opt.Ignore());
            // .ForMember(dest => dest.FieldWeatherDataSources, opt => opt.Ignore())
            // .ForMember(dest => dest.FarmId, opt => opt.Ignore());

            CreateMap<WeatherForecast, WeatherDataSourceDto>();
            //ToDo
            // .ForMember(dest => dest.Credentials, opt => opt.MapFrom((src, dest) =>
            // {
            //     var credentialsNoPassword = "";
            //     if (string.IsNullOrEmpty(src.Credentials) || src.Credentials == "null") return credentialsNoPassword;

            //     var credentialsAsObject = JsonSerializer.Deserialize<WeatherCredentials>(src.Credentials);
            //     credentialsAsObject.Password = "*******";

            //     credentialsNoPassword = JsonSerializer.Serialize(credentialsAsObject);
            //     return credentialsNoPassword;
            // }));

            CreateMap<WeatherForecast, WeatherDataSourceForCreationDto>()
                .ForMember(dest => dest.Credentials, opt => opt.Ignore());

            CreateMap<WeatherForecast, WeatherDataSourceForUpdateDto>();
            //ToDo
            // .ForMember(dest => dest.Credentials, opt => opt.MapFrom((src, dest) =>
            // {
            //     WeatherCredentials credentialsNoPassword = null;
            //     if (string.IsNullOrEmpty(src.Credentials) || src.Credentials == "null") return credentialsNoPassword;

            //     var credentialsAsObject = JsonSerializer.Deserialize<WeatherCredentials>(src.Credentials);
            //     return credentialsAsObject;
            // }));

            CreateMap<WeatherDataSourceDto, WeatherForecast>();

            CreateMap<WeatherDataSourceForCreationDto, WeatherForecast>();
            //ToDo
            // .ForMember(dest => dest.Credentials, opt => opt.MapFrom((src, dest) =>
            // {
            //     return JsonSerializer.Serialize(src.Credentials);
            // }));

            CreateMap<WeatherForManipulationDto, WeatherForecast>();
            //ToDo
            // .ForMember(dest => dest.Credentials, opt => opt.MapFrom((src, dest) =>
            // {
            //     return JsonSerializer.Serialize(src.Credentials);
            // }));

            CreateMap<WeatherDataSourceForUpdateDto, WeatherDataSourceForCreationDto>()
                .ReverseMap();

            CreateMap<WeatherForecast, WeatherSchemaForHttp>();
        }
    }
}