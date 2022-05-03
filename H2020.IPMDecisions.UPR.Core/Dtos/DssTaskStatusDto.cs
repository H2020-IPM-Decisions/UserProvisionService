using System;

namespace H2020.IPMDecisions.UPR.Core.Dtos
{
    public class DssTaskStatusDto
    {
        public Guid DssId { get; set; }
        public string Id { get; set; }
        public string JobStatus { get; set; }
        public DateTime ScheduleTime { get; set; }
        public bool IsLongWait { get; set; } = false;
    }
}