using System;
using System.ComponentModel.DataAnnotations;

namespace H2020.IPMDecisions.UPR.Core.Dtos
{
    public class UserProfileInternalCallDto
    {
        [Required]
        public Guid UserId { get; set; }
        [Required]
        public string FirstName { get; set; }       
    }
}