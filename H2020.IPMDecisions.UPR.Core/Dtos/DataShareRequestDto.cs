using System;
using System.Collections.Generic;

namespace H2020.IPMDecisions.UPR.Core.Dtos
{
    public class DataShareRequestDto
    {
        public Guid Id { get; set; }
        public Guid RequesteeId { get; set; }
        public string RequesteeName { get; set; }
        public Guid RequesterId { get; set; }
        public string RequesterName { get; set; }
        public string RequestStatus { get; set; }
        public List<Guid> AuthorizedFarms { get; set; }
    }
}