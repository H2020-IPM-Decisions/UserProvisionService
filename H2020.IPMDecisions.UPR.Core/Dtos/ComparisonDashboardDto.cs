using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace H2020.IPMDecisions.UPR.Core.Dtos
{
    public class ComparisonDashboardDto
    {
        public List<Guid> DssIds { get; set; }

        [Range(1, 30, ErrorMessage = "Days must be a value between 1 and 30.")]
        public int Days { get; set; } = 7;
    }
}