using System;
using System.ComponentModel.DataAnnotations;

namespace H2020.IPMDecisions.UPR.Core.Entities
{
    public class FieldDssResult
    {
        [Key]
        public Guid Id { get; set; }
        public DateTime CreationDate { get; set; }
        public string Result { get; set; }
        public Guid FieldCropPestDssId { get; set; }
        public FieldCropPestDss FieldCropPestDss { get; set; }
    }
}