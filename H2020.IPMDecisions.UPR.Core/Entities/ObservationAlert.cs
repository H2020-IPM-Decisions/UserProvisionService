using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace H2020.IPMDecisions.UPR.Core.Entities
{
    public class ObservationAlert
    {
        [Key]
        public Guid Id { get; set; }
        public Guid WeatherStationId { get; set; }
        public Guid CropPestDssCombinationId { get; set; }
        public CropPestDssCombination CropPestDssCombination { get; set; }
        public Guid FieldObservationId { get; set; }
        public FieldObservation FieldObservation { get; set; }

        public ICollection<ObservationResult> ObservationResults { get; set; }
    }
}