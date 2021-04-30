using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace H2020.IPMDecisions.UPR.Core.Models
{
    [NotMapped]
    public class DssResultFromDatabase
    {
        public Guid Id { get; set; }
        public string CropEppoCode { get; set; }
        public string PestEppoCode { get; set; }
        public string DssId { get; set; }
        public string DssModelId { get; set; }
        public string DssExecutionType { get; set; }
        public DateTime CreationDate { get; set; }
        public string Result { get; set; }
        public bool isValid { get; set; }
    }
}