using System;
using System.ComponentModel.DataAnnotations;

namespace H2020.IPMDecisions.UPR.Core.Entities
{
    public class ObservationResult
    {
        [Key]
        public Guid Id { get; set; }        
        public Guid ObservationAlertId { get; set; }
        public ObservationAlert ObservationAlert { get; set; }
        public DateTime Date { get; set; }
        public string Inf1 { get; set; }
        public string Inf2 { get; set; }
    }
}