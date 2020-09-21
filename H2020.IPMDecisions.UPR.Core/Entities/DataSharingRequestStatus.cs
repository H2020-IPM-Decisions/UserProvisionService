using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using H2020.IPMDecisions.UPR.Core.Enums;

namespace H2020.IPMDecisions.UPR.Core.Entities
{
    public class DataSharingRequestStatus
    {
        [Key]
        public RequestStatusEnum Id { get; set; }
        [Required]
        public string Description { get; set; }

        public List<DataSharingRequest> DataSharingRequests { get; set; }
    }
}