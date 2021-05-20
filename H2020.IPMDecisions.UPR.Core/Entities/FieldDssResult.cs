using System;
using System.ComponentModel.DataAnnotations;

namespace H2020.IPMDecisions.UPR.Core.Entities
{
    public class FieldDssResult
    {
        [Key]
        public Guid Id { get; set; }
        public DateTime CreationDate { get; set; }
        public bool IsValid { get; set; }
        public string DssFullResult { get; set; }
        public int WarningStatus { get; set; }
        public string WarningMessage { get; set; }
        public int? ResultMessageType { get; set; }
        public string ResultMessage { get; set; }
        public Guid FieldCropPestDssId { get; set; }
        public FieldCropPestDss FieldCropPestDss { get; set; }
    }
}