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
                    opt => opt.MapFrom(src => JsonConvert.DeserializeObject<dynamic>(src.DssParameters.ToString())))
                    .AfterMap((src, dest) =>
                    {
                        if (src.IsCustomDss == true && !string.IsNullOrEmpty(src.CustomName)) dest.CropPestDssDto.DssModelName = src.CustomName;
                    });

            CreateMap<FieldCropPestDss, DssParametersDto>()
                .ForMember(dest => dest.DssParameters,
                    opt => opt.MapFrom(src => JsonConvert.DeserializeObject<dynamic>(src.DssParameters.ToString())));

            // Dtos to Entities       
            CreateMap<FieldCropPestDssForUpdateDto, FieldCropPestDss>()
                .ForMember(dest => dest.DssParametersLastUpdate, opt => opt.MapFrom(src => DateTime.Now));

            CreateMap<FieldCropPestDssForAdaptationDto, FieldCropPestDss>()
                .ForMember(dest => dest.DssParameters, opt => opt.MapFrom(src => src.DssParameters))
                .ForMember(dest => dest.CustomName, opt => opt.MapFrom(src => src.Name))
                .AfterMap((src, dest, context) =>
                {
                    dest.IsCustomDss = true;
                    dest.CropPestDssId = Guid.Parse(context.Items["CropPestDssId"].ToString());
                    dest.FieldCropPestId = Guid.Parse(context.Items["FieldCropPestId"].ToString());
                });
        }
    }
}