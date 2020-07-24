using H2020.IPMDecisions.UPR.Core.ResourceParameters;

namespace H2020.IPMDecisions.UPR.Core.Profiles
{
    public class ResourceParametersProfile : MainProfile
    {
        public ResourceParametersProfile()
        {
            CreateMap<ChildrenResourceParameter, FieldResourceParameter>()
                .ForMember(src => src.Fields, opt => opt.Ignore());
        }
    }
}