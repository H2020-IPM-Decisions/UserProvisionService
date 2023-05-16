using System;
using System.Collections.Generic;

namespace H2020.IPMDecisions.UPR.Core.Models
{
    public class FarmWithDssAvailableByLocation
    {
        public Guid FarmId { get; set; }
        public IEnumerable<DssModelInformation> DssModelsAvailable { get; set; }
    }
}