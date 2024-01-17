using System.Collections.Generic;
using System.Linq;
using H2020.IPMDecisions.UPR.Core.Dtos;
using H2020.IPMDecisions.UPR.Core.Entities;
using H2020.IPMDecisions.UPR.Core.Models;

namespace H2020.IPMDecisions.UPR.Core.Profiles
{
    public class UserWeatheProfile : MainProfile
    {
        public UserWeatheProfile()
        {
            // Dtos to Entities
            CreateMap<UserWeatherForCreationDto, UserWeather>()
                .AfterMap((src, dest, context) =>
                {
                    var weatherName = context.Items["weatherName"] as string;
                    var encryptedPassword = context.Items["encryptedPassword"] as string;

                    if (string.IsNullOrEmpty(src.WeatherStationReference))
                    {
                        dest.WeatherStationReference = string.Format("{0} - Station: {1}", weatherName, src.WeatherStationId);
                    }
                    dest.Password = encryptedPassword;
                });

            CreateMap<UserWeatherForUpdateDto, UserWeather>()
               .ForMember(dest => dest.Id, opt => opt.Ignore())
               .ForMember(dest => dest.WeatherId, opt => opt.Ignore())
               .ForMember(dest => dest.UserId, opt => opt.Ignore())
               .ForMember(dest => dest.Id, opt => opt.Ignore())
               .AfterMap((src, dest, context) =>
               {
                   var encryptedPassword = context.Items["encryptedPassword"] as string;
                   dest.Password = encryptedPassword;
               });

            // Entities to Dtos
            CreateMap<UserWeather, UserWeatherDto>()
                .AfterMap((src, dest, context) =>
                {
                    dest.WeatherName = context.Items["weatherName"] as string;

                    if (context.Items.ContainsKey("farms") && context.Items["farms"] is List<FarmDto> farms)
                    {
                        dest.Farms = farms;
                    }
                });
        }
    }
}