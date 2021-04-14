using System.Collections.Generic;
using System.Linq;
using H2020.IPMDecisions.UPR.Core.Dtos;
using H2020.IPMDecisions.UPR.Core.Entities;
using NetTopologySuite.Geometries;

namespace H2020.IPMDecisions.UPR.Core.Profiles
{
    public class FarmProfile : MainProfile
    {
        public FarmProfile()
        {
            // Entities to Dtos
            CreateMap<Farm, FarmDto>()
                .ForMember(dest => dest.WeatherForecastDto, opt => opt.MapFrom(src => src.WeatherForecast))
            .AfterMap((src, dest, context) =>
                {
                    // Only select first Weather Station or Weather Data Source, because current requirements only accepts  
                    // one per farm. This might change in the future
                    // if (src.FarmWeatherDataSources.Any())
                    //     dest.WeatherDataSourceDto = context
                    //         .Mapper
                    //         .Map<WeatherDataSourceDto>(
                    //             src.FarmWeatherDataSources.FirstOrDefault());

                    if (src.FarmWeatherStations.Any())
                        dest.WeatherStationDto = context
                            .Mapper
                            .Map<WeatherStationDto>(
                                src.FarmWeatherStations.FirstOrDefault());
                });

            CreateMap<Farm, FarmWithChildrenDto>()
               .ForMember(dest => dest.FieldsDto, opt => opt.Ignore())
               .AfterMap((src, dest, context) =>
                {
                    // Only select first Weather Station or Weather Data Source, because current requirements only accepts  
                    // one per farm. This might change in the future
                    // if (src.FarmWeatherDataSources.Any())
                    //     dest.WeatherDataSourceDto = context
                    //     .Mapper
                    //     .Map<WeatherDataSourceDto>(
                    //         src.FarmWeatherDataSources.FirstOrDefault());

                    if (src.FarmWeatherStations.Any())
                        dest.WeatherStationDto = context
                        .Mapper
                        .Map<WeatherStationDto>(
                            src.FarmWeatherStations.FirstOrDefault());
                });

            CreateMap<Farm, FarmForUpdateDto>()
                .ForMember(dest => dest.WeatherForecastDto, opt => opt.MapFrom(src => src.WeatherForecast))
                .AfterMap((src, dest, context) =>
               {
                   // Only select first Weather Station or Weather Data Source, because current requirements only accepts  
                   // one per farm.This might change in the future
                   //    if (src.FarmWeatherDataSources.Any())
                   //        dest.WeatherDataSourceDto = context
                   //        .Mapper
                   //        .Map<WeatherDataSourceForUpdateDto>(
                   //            src.FarmWeatherDataSources.FirstOrDefault());

                   if (src.FarmWeatherStations.Any())
                       dest.WeatherStationDto = context
                      .Mapper
                      .Map<WeatherStationForUpdateDto>(
                          src.FarmWeatherStations.FirstOrDefault());
               }); ;

            // Dtos to Entities
            CreateMap<FarmForCreationDto, Farm>()
                .ForMember(dest => dest.WeatherForecast, opt => opt.Ignore())
                .BeforeMap((src, dest) =>
                {
                    dest.FarmWeatherStations = new List<WeatherStation>();
                })
                .AfterMap((src, dest) =>
                {
                    dest.Location = new Point(src.Location.X, src.Location.Y) { SRID = src.Location.SRID };
                });

            CreateMap<FarmForUpdateDto, Farm>()
                .AfterMap((src, dest) =>
                {
                    dest.Location = new Point(src.Location.X, src.Location.Y) { SRID = src.Location.SRID };
                });

            // Dtos to Dtos
            CreateMap<FarmForUpdateDto, FarmForCreationDto>();
        }
    }
}