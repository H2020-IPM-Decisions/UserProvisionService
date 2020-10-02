using System;
using System.ComponentModel.DataAnnotations;

namespace H2020.IPMDecisions.UPR.Core.Entities
{
    public class ForecastAlert
    {
        [Key]
        public Guid Id { get; set; }
        public Guid WeatherStationId { get; set; }
    }
}