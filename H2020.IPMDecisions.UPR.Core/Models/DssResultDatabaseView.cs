using System;

namespace H2020.IPMDecisions.UPR.Core.Models
{
    public class DssResultDatabaseView
    {
        public Guid Id { get; set; }
        public string CropEppoCode { get; set; }
        public string PestEppoCode { get; set; }
        public string DssId { get; set; }
        public string DssModelId { get; set; }
        public string DssExecutionType { get; set; }
        public DateTime CreationDate { get; set; }
        public string DssFullResult { get; set; }
        public int WarningStatus { get; set; }
        public string WarningMessage { get; set; }
        public bool isValid { get; set; }
    }
}