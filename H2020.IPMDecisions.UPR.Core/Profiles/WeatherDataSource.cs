using H2020.IPMDecisions.UPR.Core.Dtos;
using H2020.IPMDecisions.UPR.Core.Entities;

namespace H2020.IPMDecisions.UPR.Core.Profiles
{
    public class WeatherDataSourceProfile : MainProfile
    {
        public WeatherDataSourceProfile()
        {
            CreateMap<WeatherDataSource, WeatherDataSourceDto>();

            CreateMap<WeatherDataSourceDto, WeatherDataSource>()
            .ForMember(dst => dst.FileData, opt => opt.Ignore())
            .ForMember(dst => dst.FileExtension, opt => opt.Ignore())
            .ForMember(dst => dst.FileExtension, opt => opt.Ignore())
            .ForMember(dst => dst.FileUploadedOn, opt => opt.Ignore());
        }
    }
}