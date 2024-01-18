using System;
using H2020.IPMDecisions.UPR.Core.Dtos;
using H2020.IPMDecisions.UPR.Core.Entities;

namespace H2020.IPMDecisions.UPR.Core.Profiles
{
    public class FieldProfile : MainProfile
    {
        public FieldProfile()
        {
            // Entities to Dtos
            CreateMap<Field, FieldDto>();
            CreateMap<Field, FieldWithChildrenDto>();
            CreateMap<Field, FieldForUpdateDto>();

            // Dtos to Entities
            CreateMap<FieldForCreationDto, Field>();

            CreateMap<FieldForUpdateDto, Field>();

            CreateMap<FarmDssForCreationDto, Field>()
            .BeforeMap((src, dest) =>
            {
                if (string.IsNullOrEmpty(src.FieldName))
                {
                    dest.Name = string.Format("{0}_{1}_{2}_{3}",
                        "Generated_Field", src.CropEppoCode, src.PestEppoCode, src.DssId);
                }
                else
                {
                    dest.Name = src.FieldName;
                }

                if (src.FieldId == null || src.FieldId == Guid.Empty)
                {
                    dest.Id = Guid.NewGuid();
                }
                else
                {
                    dest.Id = (Guid)src.FieldId;
                }
            });

            // Dtos to Dtos
            CreateMap<FieldForUpdateDto, FieldForCreationDto>();
        }
    }
}