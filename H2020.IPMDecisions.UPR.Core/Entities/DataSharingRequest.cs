using System;
using System.ComponentModel.DataAnnotations;

namespace H2020.IPMDecisions.UPR.Core.Entities
{
    public class DataSharingRequest
    {
        [Key]
        public Guid Id { get; set; }

        public Guid RequesteeId { get; set; }
        public UserProfile Requestee { get; set; }

        public Guid RequesterId { get; set; }
        public UserProfile Requester { get; set; }

        public virtual DataSharingRequestStatus RequestStatus { get; set; }
    }
}