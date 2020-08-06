using System;
using System.ComponentModel.DataAnnotations;

namespace H2020.IPMDecisions.UPR.Core.Dtos
{
    public class UserFarmAuthorizationDto
    {
        [Required]
        public Guid FarmId { get; set; }
        [Required]
        public bool Authorize { get; set; }
    }
}