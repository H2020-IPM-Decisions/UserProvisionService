using System.Collections.Generic;
using System.Linq;
using H2020.IPMDecisions.UPR.Core.Dtos;
using H2020.IPMDecisions.UPR.Core.Models;

namespace H2020.IPMDecisions.UPR.Core.Profiles
{
    public class WeatherDataProfile : MainProfile
    {
        public WeatherDataProfile()
        {
            // Internal            
            CreateMap<WeatherDataSchema, WeatherSchemaForHttp>()
                .ForMember(dest => dest.WeatherId, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Url, opt => opt.MapFrom(src => src.EndPoint))
                .AfterMap((src, dest, context) =>
                {
                    List<string> parameterCodes = new List<string>();
                    if (src.Parameters.Common != null)
                        parameterCodes.AddRange(src.Parameters.Common.Select(s => s.ToString()).ToList());
                    if (src.Parameters.Optional != null)
                        parameterCodes.AddRange(src.Parameters.Optional.Select(s => s.ToString()).ToList());
                    dest.WeatherParameters = parameterCodes;

                    dest.Url = dest.Url.Replace("{WEATHER_API_URL}", context.Items["host"].ToString());
                });

            // Entities to Dtos
            CreateMap<WeatherDataSchema, WeatherBaseDto>()
                .ForMember(dest => dest.WeatherId, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Url, opt => opt.MapFrom(src => src.EndPoint))
                .AfterMap((src, dest, context) =>
                {
                    dest.Url = dest.Url.Replace("{WEATHER_API_URL}", context.Items["host"].ToString());
                });
        }
    }
}