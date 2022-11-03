using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using H2020.IPMDecisions.UPR.Core.Enums;

namespace H2020.IPMDecisions.UPR.Core.Dtos
{
    public class DataShareRequestUpdateDto
    {
        [Required]
        public Guid RequesterId { get; set; }
        public List<UserFarmAuthorizationDto> Farms { get; set; }
        [Required]
        [EnumDataType(typeof(RequestStatusEnum))]
        public string Reply { get; set; }
    }
}