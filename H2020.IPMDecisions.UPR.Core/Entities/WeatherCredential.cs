using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace H2020.IPMDecisions.UPR.Core.Entities
{
    public class WeatherCredential
    {
        [Key]
        public Guid Id { get; set; }
        public string UserName { get; set; }
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        public string WeatherStationId { get; set; }

        public ICollection<Farm> Farms { get; set; }
    }
}