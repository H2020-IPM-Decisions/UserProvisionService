using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace H2020.IPMDecisions.UPR.Core.Dtos
{
    public class ComparisonDashboardDto
    {
        public List<Guid> DssIds { get; set; }

        public int Days { get; set; }
    }
}