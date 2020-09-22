using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace H2020.IPMDecisions.UPR.Core.Entities
{
    public class CropPestDssCombination
    {
        [Key]
        public Guid Id { get; set; }
        public Guid CropPestId { get; set; }
        public CropPest CropPest { get; set; }
        public string DssName { get; set; }
        public ICollection<ForecastAlert> ForecastAlerts { get; set; }
        public ICollection<ObservationAlert> ObservationAlerts { get; set; }
    }
}