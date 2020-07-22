using System;
using H2020.IPMDecisions.UPR.Core.Models;

namespace H2020.IPMDecisions.UPR.Core.Dtos
{
    public class FieldObservationDto
    {
        public Guid Id { get; set; }
        public CustomPointLocation Location { get; set; }
        public DateTime Time { get; set; }
        public string PestEppoCode { get; set; }
        public string CropEppoCode { get; set; }
    }
}