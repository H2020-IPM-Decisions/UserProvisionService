using System;
using System.ComponentModel.DataAnnotations;

namespace H2020.IPMDecisions.UPR.Core.Entities
{
    public class FieldCropDecisionCombination
    {
        [Key]
        public Guid Id { get; set; }
        // public Guid FielId { get; set; }
        // public Field Field { get; set; }

        // public Guid CropDecisionCombinationId { get; set; }
        // public CropDecisionCombination CropDecisionCombination { get; set; }
    }
}