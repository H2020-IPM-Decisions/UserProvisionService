using System;
using System.Linq;
using H2020.IPMDecisions.UPR.Core.Dtos;
using H2020.IPMDecisions.UPR.Core.Entities;
using H2020.IPMDecisions.UPR.Core.Enums;
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
                .ForMember(dest => dest.WeatherHistoricalDto, opt => opt.MapFrom(src => src.WeatherHistorical))
                .AfterMap((src, dest, context) =>
                {
                    var userId = context.TryGetItems(out var items) ?  Guid.Parse(items["UserId"].ToString()) : new Guid();
                    dest.IsShared = src.UserFarms.Count > 1 ? true : false;
                    if (dest.IsShared)
                    {
                        // get advisor farm
                        var advisorProfile = src.UserFarms
                            .Where(uf => uf.UserFarmType.Description.Equals(UserFarmTypeEnum.Advisor.ToString(), StringComparison.InvariantCultureIgnoreCase))
                            .FirstOrDefault().UserProfile;
                        dest.SharedPersonName = $"{advisorProfile.FirstName} {advisorProfile.LastName}";
                        // get owner farm
                        var ownerProfile = src.UserFarms
                            .Where(uf => uf.UserFarmType.Description.Equals(UserFarmTypeEnum.Owner.ToString(), StringComparison.InvariantCultureIgnoreCase))
                            .FirstOrDefault().UserProfile;
                        if (!userId.Equals(ownerProfile.UserId)) dest.Owner = false;
                    }
                });

            CreateMap<Farm, FarmWithChildrenDto>()
               .ForMember(dest => dest.FieldsDto, opt => opt.Ignore());

            CreateMap<Farm, FarmForUpdateDto>()
                .ForMember(dest => dest.WeatherForecastDto, opt => opt.MapFrom(src => src.WeatherForecast))
                .ForMember(dest => dest.WeatherHistoricalDto, opt => opt.MapFrom(src => src.WeatherHistorical));

            // Dtos to Entities
            CreateMap<FarmForCreationDto, Farm>()
                .ForMember(dest => dest.WeatherForecast, opt => opt.Ignore())
                .ForMember(dest => dest.WeatherHistorical, opt => opt.Ignore())
                .AfterMap((src, dest) =>
                {
                    dest.Location = new Point(Math.Round(src.Location.X, 4), Math.Round(src.Location.Y, 4)) { SRID = src.Location.SRID };
                });

            CreateMap<FarmForUpdateDto, Farm>()
                .AfterMap((src, dest) =>
                {
                    dest.Location = new Point(Math.Round(src.Location.X, 4), Math.Round(src.Location.Y, 4)) { SRID = src.Location.SRID };
                });

            CreateMap<FarmForFullUpdateDto, Farm>()
                .AfterMap((src, dest) =>
                {
                    dest.Location = new Point(Math.Round(src.Location.X, 4), Math.Round(src.Location.Y, 4)) { SRID = src.Location.SRID };
                });

            // Dtos to Dtos
            CreateMap<FarmForUpdateDto, FarmForCreationDto>();
        }
    }
}