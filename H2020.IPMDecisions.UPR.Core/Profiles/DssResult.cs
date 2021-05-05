using System;
using System.Linq;
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
            CreateMap<FieldCropPestDss, FieldDssResultDto>()
                .ForMember(dest => dest.CropEppoCode, opt => opt.MapFrom(src => src.FieldCropPest.CropPest.CropEppoCode))
                .ForMember(dest => dest.PestEppoCode, opt => opt.MapFrom(src => src.FieldCropPest.CropPest.PestEppoCode))
                .ForMember(dest => dest.DssId, opt => opt.MapFrom(src => src.CropPestDss.DssId))
                .ForMember(dest => dest.DssModelId, opt => opt.MapFrom(src => src.CropPestDss.DssModelId))
                .ForMember(dest => dest.DssExecutionType, opt => opt.MapFrom(src => src.CropPestDss.DssExecutionType))
                .ForMember(dest => dest.IsValid,
                    opt => opt.MapFrom(src => src.FieldDssResults.OrderByDescending(r => r.CreationDate).FirstOrDefault().IsValid))
                .ForMember(dest => dest.CreationDate,
                    opt => opt.MapFrom(src => src.FieldDssResults.OrderByDescending(r => r.CreationDate).FirstOrDefault().CreationDate))
                .ForMember(dest => dest.WarningStatus,
                    opt => opt.MapFrom(src => src.FieldDssResults.OrderByDescending(r => r.CreationDate).FirstOrDefault().WarningStatus))
                .ForMember(dest => dest.DssFullResult,
                    opt => opt.MapFrom(src => src.FieldDssResults.OrderByDescending(r => r.CreationDate).FirstOrDefault().DssFullResult))
                .AfterMap((src, dest) =>
                {
                    switch (dest.WarningStatus)
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

            CreateMap<FieldCropPestDss, FieldDssResultDetailedDto>()
                .ForMember(dest => dest.CropEppoCode, opt => opt.MapFrom(src => src.FieldCropPest.CropPest.CropEppoCode))
                .ForMember(dest => dest.PestEppoCode, opt => opt.MapFrom(src => src.FieldCropPest.CropPest.PestEppoCode))
                .ForMember(dest => dest.DssId, opt => opt.MapFrom(src => src.CropPestDss.DssId))
                .ForMember(dest => dest.DssModelId, opt => opt.MapFrom(src => src.CropPestDss.DssModelId))
                .ForMember(dest => dest.DssExecutionType, opt => opt.MapFrom(src => src.CropPestDss.DssExecutionType))
                .ForMember(dest => dest.IsValid,
                    opt => opt.MapFrom(src => src.FieldDssResults.OrderByDescending(r => r.CreationDate).FirstOrDefault().IsValid))
                .ForMember(dest => dest.CreationDate,
                    opt => opt.MapFrom(src => src.FieldDssResults.OrderByDescending(r => r.CreationDate).FirstOrDefault().CreationDate))
                .ForMember(dest => dest.WarningStatus,
                    opt => opt.MapFrom(src => src.FieldDssResults.OrderByDescending(r => r.CreationDate).FirstOrDefault().WarningStatus))
                .ForMember(dest => dest.DssFullResult,
                    opt => opt.MapFrom(src => src.FieldDssResults.OrderByDescending(r => r.CreationDate).FirstOrDefault().DssFullResult))
                .AfterMap((src, dest) =>
                {
                    switch (dest.WarningStatus)
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