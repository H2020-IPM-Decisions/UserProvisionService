using H2020.IPMDecisions.UPR.Core.Dtos;
using H2020.IPMDecisions.UPR.Core.Entities;

namespace H2020.IPMDecisions.UPR.Core.Profiles
{
    public class DataShareProfile: MainProfile
    {
        public DataShareProfile()
        {
            // Entities to Dtos
            CreateMap<DataSharingRequest, DataShareRequestDto>()
                .ForMember(dest => 
                    dest.RequesteeName, 
                    opt => opt.MapFrom(src => $"{src.Requestee.FirstName} {src.Requestee.LastName}"))
                .ForMember(dest =>
                    dest.RequesterName,
                    opt => opt.MapFrom(src => $"{src.Requester.FirstName} {src.Requester.LastName}"))
                .ForMember(dest =>
                    dest.RequestStatus,
                    opt => opt.MapFrom(src => src.RequestStatus.Description))
                .AfterMap((src, dest) => dest.RequesteeName = dest.RequesteeName.Trim())
                .AfterMap((src, dest) => dest.RequesterName = dest.RequesterName.Trim())
                .AfterMap((src, dest) => dest.RequestStatus = dest.RequestStatus.ToString());
        }        
    }
}