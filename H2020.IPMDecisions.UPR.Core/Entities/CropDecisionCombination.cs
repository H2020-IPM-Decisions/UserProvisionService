using System;

namespace H2020.IPMDecisions.UPR.Core.Entities
{
    public class CropDecisionCombination
    {
        public Guid CropId { get; set; }
        public Crop Crop { get; set; }

        public Guid DssId { get; set; }
        public Dss Dss { get; set; }

        public Guid PestId { get; set; }
        public Pest Pest { get; set; }

        public string Inf1 { get; set; }
    }
}