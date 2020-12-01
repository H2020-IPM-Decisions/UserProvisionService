using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using H2020.IPMDecisions.UPR.Core.Enums;

namespace H2020.IPMDecisions.UPR.Core.Dtos
{
    public class DataShareRequestReplyDto
    {
        [Required]
        public Guid RequesterId { get; set; }
        public List<Guid> Farms { get; set; }
        [Required]
        [EnumDataType(typeof(RequestStatusEnum))]
        public string Reply { get; set; }
        public bool AllowAllFarms { get; set; } = false;
    }
}