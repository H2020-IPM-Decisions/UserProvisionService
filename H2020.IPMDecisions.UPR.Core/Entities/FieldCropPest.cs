using System;
using System.ComponentModel.DataAnnotations;

namespace H2020.IPMDecisions.UPR.Core.Entities
{
    public class FieldCropPest
    {
        [Key]
        public Guid Id { get; set; }
        public Guid FieldId { get; set; }
        public Field Field { get; set; }
        public Guid CropPestId { get; set; }
        public CropPest CropPest { get; set; }
    }
}