using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace H2020.IPMDecisions.UPR.Core.Entities
{
    public class CropDecisionCombination
    {
        [Key]
        public Guid Id { get; set; }
        
        public Guid CropId { get; set; }
        public Crop Crop { get; set; }

        public Guid DssId { get; set; }
        public Dss Dss { get; set; }

        public Guid PestId { get; set; }
        public Pest Pest { get; set; }

        public string Inf1 { get; set; }

        public IList<FieldCropDecisionCombination> FieldCropDecisionCombinations { get; set; }
    }
}