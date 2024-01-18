using System.ComponentModel.DataAnnotations;

namespace H2020.IPMDecisions.UPR.Core.Dtos
{
    public class UserProfileForManipulationDto
    {
        [Required(ErrorMessage = "First Name is required")]
        [MaxLength(80, ErrorMessage = "First Name max length 80 characters")]
        public virtual string FirstName { get; set; }
        public virtual string LastName { get; set; }
        public virtual string PhoneNumber { get; set; }
        public virtual string MobileNumber { get; set; }
        public virtual string Street { get; set; }
        public virtual string City { get; set; }
        public virtual string Postcode { get; set; }
        public virtual string Country { get; set; }
    }
}