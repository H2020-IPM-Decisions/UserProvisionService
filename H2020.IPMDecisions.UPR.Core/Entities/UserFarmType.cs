using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using H2020.IPMDecisions.UPR.Core.Enums;

namespace H2020.IPMDecisions.UPR.Core.Entities
{
    public class UserFarmType
    {
        [Key]
        public UserFarmTypeEnum Id { get; set; }
        [Required]
        public string Description { get; set; }

        public List<UserFarm> UserFarms { get; set; }
    }
}