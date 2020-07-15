using System;
using System.ComponentModel.DataAnnotations;

namespace H2020.IPMDecisions.UPR.Core.Entities
{
    public class UserFarm
    {
        public Guid UserId { get; set; }
        public UserProfile UserProfile { get; set; }
        
        public Guid FarmId { get; set; }
        public Farm Farm { get; set; }
    }
}