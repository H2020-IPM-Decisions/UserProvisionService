using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace H2020.IPMDecisions.UPR.Core.Entities
{
    public class UserWeather
    {
        [Key]
        public Guid Id { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string WeatherId { get; set; }
        public string WeatherStationId { get; set; }
        public string WeatherStationReference { get; set; }

        public Guid UserId { get; set; }
        public UserProfile UserProfile { get; set; }
    }
}