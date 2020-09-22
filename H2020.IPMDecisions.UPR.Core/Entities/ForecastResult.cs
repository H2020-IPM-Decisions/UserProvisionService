using System;
using System.ComponentModel.DataAnnotations;

namespace H2020.IPMDecisions.UPR.Core.Entities
{
    public class ForecastResult
    {
        [Key]
        public Guid Id { get; set; }        
        public Guid ForecastAlertId { get; set; }
        public ForecastAlert ForecastAlert { get; set; }
        public DateTime Date { get; set; }
        public string Inf1 { get; set; }
        public string Inf2 { get; set; }
    }
}