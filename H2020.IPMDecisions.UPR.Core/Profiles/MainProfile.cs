using AutoMapper;

namespace H2020.IPMDecisions.UPR.Core.Profiles
{
    public class MainProfile : Profile
    {
        public MainProfile()
        {
            CreateMap<string, string>()
                .ConvertUsing(str => (str ?? "").Trim());
        }
    }
}