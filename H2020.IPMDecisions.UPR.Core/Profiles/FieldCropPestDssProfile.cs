using System;
using System.Linq;
using H2020.IPMDecisions.UPR.Core.Dtos;
using H2020.IPMDecisions.UPR.Core.Entities;
using Newtonsoft.Json;

namespace H2020.IPMDecisions.UPR.Core.Profiles
{
    public class FieldCropPestDssProfile : MainProfile
    {
        public FieldCropPestDssProfile()
        {
            // Entities to Dtos
            CreateMap<FieldCropPestDss, FieldCropPestDssDto>()
                .ForMember(dest => dest.CropPestDssDto, opt => opt.MapFrom(src => src.CropPestDss))
                .ForMember(dest => dest.FieldCropPest, opt => opt.MapFrom(src => src.FieldCropPest))
                .ForMember(dest => dest.DssResult,
                    opt => opt.MapFrom(src => src.FieldDssResults.OrderByDescending(r => r.CreationDate).FirstOrDefault()))
                .ForMember(dest => dest.DssParameters,
                    opt => opt.MapFrom(src => JsonConvert.DeserializeObject<dynamic>(src.DssParameters.ToString())));

            CreateMap<FieldCropPestDss, DssParametersDto>()
                .ForMember(dest => dest.DssParameters,
                    opt => opt.MapFrom(src => JsonConvert.DeserializeObject<dynamic>(src.DssParameters.ToString())));

            // Dtos to Entities       
            CreateMap<FieldCropPestDssForUpdateDto, FieldCropPestDss>();

            CreateMap<FieldCropPestDssForAdaptationDto, FieldCropPestDss>()
                .ForMember(dest => dest.DssParameters, opt => opt.MapFrom(src => src.DssParameters))
                .ForMember(dest => dest.CustomName, opt => opt.MapFrom(src => src.Name))
                .AfterMap((src, dest, context) =>
                {
                    dest.IsCustomDss = true;
                    dest.CropPestDssId = Guid.Parse(context.Options.Items["CropPestDssId"].ToString());
                    dest.FieldCropPestId = Guid.Parse(context.Options.Items["FieldCropPestId"].ToString());
                });
        }
    }
}