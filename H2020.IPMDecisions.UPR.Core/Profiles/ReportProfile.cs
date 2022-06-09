using H2020.IPMDecisions.UPR.Core.Dtos;
using H2020.IPMDecisions.UPR.Core.Entities;
using H2020.IPMDecisions.UPR.Core.Models;

namespace H2020.IPMDecisions.UPR.Core.Profiles
{
    public class ReportProfile : MainProfile
    {
        public ReportProfile()
        {
            // Entities to Models 
            CreateMap<CropPestDss, ReportDataDssModel>()
                .ForMember(dest => dest.ModelId, opt => opt.MapFrom(src => src.DssModelId))
                .ForMember(dest => dest.ModelName, opt => opt.MapFrom(src => src.DssModelName));

            CreateMap<Farm, ReportDataFarm>()
                .AfterMap((src, dest) =>
                {
                    dest.Location = new CustomPointLocation(src.Location.X, src.Location.Y, src.Location.SRID);
                });

            CreateMap<UserFarm, ReportData>()
                .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.UserId))
                .ForMember(dest => dest.Farm, opt => opt.MapFrom(src => src.Farm));
        }
    }
}