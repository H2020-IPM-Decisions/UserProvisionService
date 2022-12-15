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
                .ForMember(dest => dest.DssName, opt => opt.MapFrom(src => src.CropPestDss.DssName))
                .ForMember(dest => dest.DssModelId, opt => opt.MapFrom(src => src.CropPestDss.DssModelId))
                .ForMember(dest => dest.DssModelName, opt => opt.MapFrom(src => src.CropPestDss.DssModelName))
                .ForMember(dest => dest.DssExecutionType, opt => opt.MapFrom(src => src.CropPestDss.DssExecutionType))
                .ForMember(dest => dest.FarmId, opt => opt.MapFrom(src => src.FieldCropPest.FieldCrop.Field.FarmId))
                .ForMember(dest => dest.FarmName, opt => opt.MapFrom(src => src.FieldCropPest.FieldCrop.Field.Farm.Name))
                .ForMember(dest => dest.FieldId, opt => opt.MapFrom(src => src.FieldCropPest.FieldCrop.Field.Id))
                .ForPath(dest => dest.DssTaskStatusDto.Id, opt => opt.MapFrom(src => src.LastJobId))
                .ForMember(dest => dest.IsValid,
                    opt => opt.MapFrom(src => src.FieldDssResults.OrderByDescending(r => r.CreationDate).FirstOrDefault().IsValid))
                .ForMember(dest => dest.CreationDate,
                    opt => opt.MapFrom(src => src.FieldDssResults.OrderByDescending(r => r.CreationDate).FirstOrDefault().CreationDate))
                .ForMember(dest => dest.WarningStatus,
                    opt => opt.MapFrom(src => src.FieldDssResults.OrderByDescending(r => r.CreationDate).FirstOrDefault().WarningStatus))
                .ForMember(dest => dest.DssFullResult,
                    opt => opt.MapFrom(src => src.FieldDssResults.OrderByDescending(r => r.CreationDate).FirstOrDefault().DssFullResult))
                .ForMember(dest => dest.ResultMessageType,
                    opt => opt.MapFrom(src => src.FieldDssResults.OrderByDescending(r => r.CreationDate).FirstOrDefault().ResultMessageType))
                .ForMember(dest => dest.ResultMessage,
                    opt => opt.MapFrom(src => src.FieldDssResults.OrderByDescending(r => r.CreationDate).FirstOrDefault().ResultMessage))
                .AfterMap((src, dest) =>
                {
                    if (src.CropPestDss.DssExecutionType.ToLower() == "link") dest.IsValid = true;
                });

            CreateMap<FieldCropPestDss, FieldDssResultDetailedDto>()
                .ForMember(dest => dest.CropEppoCode, opt => opt.MapFrom(src => src.FieldCropPest.CropPest.CropEppoCode))
                .ForMember(dest => dest.PestEppoCode, opt => opt.MapFrom(src => src.FieldCropPest.CropPest.PestEppoCode))
                .ForMember(dest => dest.DssId, opt => opt.MapFrom(src => src.CropPestDss.DssId))
                .ForMember(dest => dest.DssName, opt => opt.MapFrom(src => src.CropPestDss.DssName))
                .ForMember(dest => dest.DssModelId, opt => opt.MapFrom(src => src.CropPestDss.DssModelId))
                .ForMember(dest => dest.DssModelName, opt => opt.MapFrom(src => src.CropPestDss.DssModelName))
                .ForMember(dest => dest.DssModelVersion, opt => opt.MapFrom(src => src.CropPestDss.DssModelVersion))
                .ForMember(dest => dest.DssExecutionType, opt => opt.MapFrom(src => src.CropPestDss.DssExecutionType))
                .ForMember(dest => dest.FarmId, opt => opt.MapFrom(src => src.FieldCropPest.FieldCrop.Field.FarmId))
                .ForMember(dest => dest.FarmName, opt => opt.MapFrom(src => src.FieldCropPest.FieldCrop.Field.Farm.Name))
                .ForMember(dest => dest.FieldId, opt => opt.MapFrom(src => src.FieldCropPest.FieldCrop.Field.Id))
                .ForMember(dest => dest.IsValid,
                    opt => opt.MapFrom(src => src.FieldDssResults.OrderByDescending(r => r.CreationDate).FirstOrDefault().IsValid))
                .ForMember(dest => dest.CreationDate,
                    opt => opt.MapFrom(src => src.FieldDssResults.OrderByDescending(r => r.CreationDate).FirstOrDefault().CreationDate))
                .ForMember(dest => dest.WarningStatus,
                    opt => opt.MapFrom(src => src.FieldDssResults.OrderByDescending(r => r.CreationDate).FirstOrDefault().WarningStatus))
                .ForMember(dest => dest.DssFullResult,
                    opt => opt.MapFrom(src => src.FieldDssResults.OrderByDescending(r => r.CreationDate).FirstOrDefault().DssFullResult))
                .ForMember(dest => dest.ResultMessageType,
                    opt => opt.MapFrom(src => src.FieldDssResults.OrderByDescending(r => r.CreationDate).FirstOrDefault().ResultMessageType))
                .ForMember(dest => dest.ResultMessage,
                    opt => opt.MapFrom(src => src.FieldDssResults.OrderByDescending(r => r.CreationDate).FirstOrDefault().ResultMessage))
                .AfterMap((src, dest) =>
                {
                    if (src.CropPestDss.DssExecutionType.ToLower() == "link") dest.IsValid = true;
                });

            // Dtos to Entities
            CreateMap<FieldDssResultForCreationDto, FieldDssResult>()
                .ForMember(dest => dest.CreationDate, opt => opt.MapFrom(src => DateTime.Now));

            // Models to Dtos
            CreateMap<DssResultDatabaseView, FieldDssResultDto>()
                .ForPath(dest => dest.DssTaskStatusDto.Id, opt => opt.MapFrom(src => src.LastJobId))
                .AfterMap((src, dest) =>
                {
                    if (src.DssExecutionType.ToLower() == "link") dest.IsValid = true;
                });

            CreateMap<OutputChartInfo, DssParameterChartInformation>();

            CreateMap<DssInformation, FieldDssResultDto>()
                .ForMember(dest => dest.DssName, opt => opt.MapFrom(src => src.Name))
                .ForMember(dest => dest.DssLogoUrl, opt => opt.MapFrom(src => src.LogoUrl))
                .ForMember(dest => dest.DssSource, opt => opt.MapFrom(src =>
                            string.Format("{0}, {1}",
                                src.DssOrganization.Name,
                                src.DssOrganization.Country)))
                .ForMember(dest => dest.Id, opt => opt.Ignore());

            CreateMap<DssModelInformation, FieldDssResultDto>()
                .ForMember(dest => dest.DssModelName, opt => opt.MapFrom(src => src.Name))
                .ForMember(dest => dest.DssDescription, opt => opt.MapFrom(src => src.Description))
                .ForMember(dest => dest.DssPurpose, opt => opt.MapFrom(src => src.Purpose))
                .ForMember(dest => dest.ValidatedSpatialCountries, opt => opt.MapFrom(src => src.ValidSpatial.Countries))
                .ForMember(dest => dest.Id, opt => opt.Ignore());

            CreateMap<DssModelInformation, CropPestDssDto>()
               .ForMember(dest => dest.DssModelName, opt => opt.MapFrom(src => src.Name))
               .ForMember(dest => dest.DssPurpose, opt => opt.MapFrom(src => src.Purpose))
               .ForMember(dest => dest.ValidatedSpatialCountries, opt => opt.MapFrom(src => src.ValidSpatial.Countries))
               .ForMember(dest => dest.Id, opt => opt.Ignore());

            CreateMap<DssModelInformation, FieldDssResultDetailedDto>()
               .ForMember(dest => dest.DssModelName, opt => opt.MapFrom(src => src.Name))
               .ForMember(dest => dest.DssTypeOfDecision, opt => opt.MapFrom(src => src.TypeOfDecision))
               .ForMember(dest => dest.DssTypeOfOutput, opt => opt.MapFrom(src => src.TypeOfOutput))
               .ForMember(dest => dest.DssDescription, opt => opt.MapFrom(src => src.Description))
               .ForMember(dest => dest.DssPurpose, opt => opt.MapFrom(src => src.Purpose))
               .ForMember(dest => dest.ValidatedSpatialCountries, opt => opt.MapFrom(src => src.ValidSpatial.Countries))
               .ForMember(dest => dest.Id, opt => opt.Ignore());
        }
    }
}