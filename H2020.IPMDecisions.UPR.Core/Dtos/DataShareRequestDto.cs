using System;
using H2020.IPMDecisions.UPR.Core.Enums;

namespace H2020.IPMDecisions.UPR.Core.Dtos
{
    public class DataShareRequestDto
    {
        public Guid RequesteeId { get; set; }
        public string RequesteeName { get; set; }
        public Guid RequesterId { get; set; }
        public string RequesterName { get; set; }
        public string RequestStatus { get; set; }
    }
}