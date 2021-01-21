using H2020.IPMDecisions.UPR.Core.ResourceParameters;

namespace H2020.IPMDecisions.UPR.Core.Profiles
{
    public class ResourceParametersProfile : MainProfile
    {
        public ResourceParametersProfile()
        {
            CreateMap<FarmResourceParameter, FieldResourceParameter>()
                .ForMember(dest => dest.Fields, opt => opt.Ignore())
                .ForMember(dest => dest.OrderBy, opt => opt.Ignore())
                .ForMember(dest => dest.SearchQuery, opt => opt.Ignore())
                .ForMember(dest => dest.PageNumber, opt => opt.MapFrom(src => src.ChildPageNumber))
                .ForMember(dest => dest.PageSize, opt => opt.MapFrom(src => src.ChildPageSize));

            CreateMap<FieldResourceParameter, FieldObservationResourceParameter>()
                .ForMember(dest => dest.Fields, opt => opt.Ignore())
                .ForMember(dest => dest.OrderBy, opt => opt.Ignore())
                .ForMember(dest => dest.SearchQuery, opt => opt.Ignore())
                .ForMember(dest => dest.PageNumber, opt => opt.MapFrom(src => src.ChildPageNumber))
                .ForMember(dest => dest.PageSize, opt => opt.MapFrom(src => src.ChildPageSize));

            CreateMap<FieldResourceParameter, FieldCropPestResourceParameter>()
                .ForMember(dest => dest.Fields, opt => opt.Ignore())
                .ForMember(dest => dest.OrderBy, opt => opt.Ignore())
                .ForMember(dest => dest.SearchQuery, opt => opt.Ignore())
                .ForMember(dest => dest.PageNumber, opt => opt.MapFrom(src => src.ChildPageNumber))
                .ForMember(dest => dest.PageSize, opt => opt.MapFrom(src => src.ChildPageSize));

            CreateMap<FieldResourceParameter, FieldSprayResourceParameter>()
                .ForMember(dest => dest.Fields, opt => opt.Ignore())
                .ForMember(dest => dest.OrderBy, opt => opt.Ignore())
                .ForMember(dest => dest.SearchQuery, opt => opt.Ignore())
                .ForMember(dest => dest.PageNumber, opt => opt.MapFrom(src => src.ChildPageNumber))
                .ForMember(dest => dest.PageSize, opt => opt.MapFrom(src => src.ChildPageSize));
        }
    }
}