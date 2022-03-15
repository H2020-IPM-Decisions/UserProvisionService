using H2020.IPMDecisions.UPR.Core.Dtos;
using Hangfire.Storage.Monitoring;

namespace H2020.IPMDecisions.UPR.Core.Profiles
{
    public class TasksProfile : MainProfile
    {
        public TasksProfile()
        {
            // Dtos to Dtos
            CreateMap<StateHistoryDto, DssTaskStatusDto>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.StateName))
                .ForMember(dest => dest.JobStatus, opt => opt.MapFrom(src => src.StateName));
        }
    }
}