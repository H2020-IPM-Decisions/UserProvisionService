using System;

namespace H2020.IPMDecisions.UPR.Core.Dtos
{
    public class DssHistoricalDataTask
    {
        public string TaskType { get; set; }
        public DssTaskStatusDto TaskStatusDto { get; set; }
    }
}