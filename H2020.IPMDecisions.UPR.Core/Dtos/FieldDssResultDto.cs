using System;

namespace H2020.IPMDecisions.UPR.Core.Dtos
{
    public class FieldDssResultDto
    {
        public Guid Id { get; set; }
        public DateTime CreationDate { get; set; }
        public string Result { get; set; }
    }
}