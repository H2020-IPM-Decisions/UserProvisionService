using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace H2020.IPMDecisions.UPR.Core.Entities
{
    public class Dss
    {
        [Key]
        public Guid Id { get; set; }
        [Required]
        public string Name { get; set; }

        public IList<CropDecisionCombination> CropDecisionCombinations { get; set; }
    }
}