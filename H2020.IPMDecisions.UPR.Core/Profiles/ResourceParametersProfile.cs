using H2020.IPMDecisions.UPR.Core.ResourceParameters;

namespace H2020.IPMDecisions.UPR.Core.Profiles
{
    public class ResourceParametersProfile : MainProfile
    {
        public ResourceParametersProfile()
        {
            CreateMap<ChildrenResourceParameter, FieldResourceParameter>()
                .ForMember(dest => dest.Fields, opt => opt.Ignore());

            CreateMap<ChildrenResourceParameter, FieldObservationResourceParameter>()
                .ForMember(dest => dest.Fields, opt => opt.Ignore());

            CreateMap<FarmResourceParameter, FieldResourceParameter>()
               .ForMember(dest => dest.PageNumber, opt => opt.MapFrom(src => src.ChildPageNumber))
               .ForMember(dest => dest.PageSize, opt => opt.MapFrom(src => src.ChildPageSize));
        }
    }
}