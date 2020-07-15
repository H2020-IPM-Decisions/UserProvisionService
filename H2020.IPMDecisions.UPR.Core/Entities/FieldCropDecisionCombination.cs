using System;

namespace H2020.IPMDecisions.UPR.Core.Entities
{
    public class FieldCropDecisionCombination
    {
        public Guid FielId { get; set; }
        public Field Field { get; set; }

        public Guid CropDecisionCombinationId { get; set; }
        public CropDecisionCombination CropDecisionCombination { get; set; }
    }
}