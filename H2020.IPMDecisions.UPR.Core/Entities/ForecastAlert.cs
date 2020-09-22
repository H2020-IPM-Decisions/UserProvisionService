using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace H2020.IPMDecisions.UPR.Core.Entities
{
    public class ForecastAlert
    {
        [Key]
        public Guid Id { get; set; }
        public Guid WeatherStationId { get; set; }
        public Guid CropPestDssCombinationId { get; set; }
        public CropPestDssCombination CropPestDssCombination { get; set; }

        public ICollection<ForecastResult> ForecastResults { get; set; }
    }
}