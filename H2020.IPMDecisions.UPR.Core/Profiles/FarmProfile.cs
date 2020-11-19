using System.Collections.Generic;
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
            CreateMap<Farm, FarmDto>();
            CreateMap<Farm, FarmWithChildrenDto>()
               .ForMember(dest => dest.FieldsDto, opt => opt.Ignore());
            CreateMap<Farm, FarmForUpdateDto>();

            // Dtos to Entities
            CreateMap<FarmForCreationDto, Farm>()
                .BeforeMap((src, dest) => 
                {
                    dest.FarmWeatherStations = new List<FarmWeatherStation>() 
                    { 
                        new FarmWeatherStation() 
                        {
                            Farm = dest,
                            WeatherStationId = src.WeatherStationDto.Id
                        }
                    };
                    dest.FarmWeatherDataSources = new List<FarmWeatherDataSource>()
                    {
                        new FarmWeatherDataSource()
                        {
                            Farm = dest,
                            WeatherDataSourceId = src.WeatherDataSourceDto.Id
                        }
                    };
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