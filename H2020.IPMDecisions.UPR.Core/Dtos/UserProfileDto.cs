using System;

namespace H2020.IPMDecisions.UPR.Core.Dtos
{
    public class UserProfileDto
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public string FullName { get; set; }
        public string PhoneNumber { get; set; }
        public string MobileNumber { get; set; }
        public string Address { get; set; }
        public string Postcode { get; set; }
        public string Country { get; set; }
    }
}