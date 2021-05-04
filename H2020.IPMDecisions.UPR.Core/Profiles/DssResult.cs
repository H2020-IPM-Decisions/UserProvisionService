using System;
using H2020.IPMDecisions.UPR.Core.Dtos;
using H2020.IPMDecisions.UPR.Core.Entities;
using H2020.IPMDecisions.UPR.Core.Models;

namespace H2020.IPMDecisions.UPR.Core.Profiles
{
    public class DssResultProfile : MainProfile
    {
        public DssResultProfile()
        {
            // Entities to Dtos
            CreateMap<FieldDssResult, FieldDssResultDto>();

            // Dtos to Entities
            CreateMap<FieldDssResultForCreationDto, FieldDssResult>()
                .ForMember(dest => dest.CreationDate, opt => opt.MapFrom(src => DateTime.Now));

            // Models to Dtos
            CreateMap<DssResultDatabaseView, FieldDssResultDto>()
                .AfterMap((src, dest) =>
                {
                    switch (src.WarningStatus)
                    {
                        // Representation from /api/dss/rest/schema/modeloutput
                        case 1:
                            dest.WarningStatusRepresentation = "Cannot give status due to error";
                            break;
                        case 2:
                            dest.WarningStatusRepresentation = "No risk of infection";
                            break;
                        case 3:
                            dest.WarningStatusRepresentation = "Medium risk of infection";
                            break;
                        case 4:
                            dest.WarningStatusRepresentation = "High risk of infection";
                            break;
                        case 0:
                        default:
                            dest.WarningStatusRepresentation = "Status gives no meaning (e.g. outside of season or before biofix)";
                            break;
                    }
                });
        }
    }
}