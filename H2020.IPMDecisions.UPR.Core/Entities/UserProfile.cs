using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace H2020.IPMDecisions.UPR.Core.Entities
{
    public class UserProfile
    {
        [Key]
        public Guid UserId { get; set; }
        [Required]
        [MaxLength(80)]
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string PhoneNumber { get; set; }
        public string MobileNumber { get; set; }
        public Guid? UserAddressId { get; set; }
        public UserAddress UserAddress { get; set; }

        public IList<UserFarm> UserFarms { get; set; }
        public ICollection<UserWidget> UserWidgets { get; set; }
        public ICollection<DataSharingRequest> DataSharingRequests { get; set; }
    }
}
