using System.ComponentModel.DataAnnotations;

namespace H2020.IPMDecisions.UPR.Core.Dtos
{
    public class DssResultListFilterDto
    {
        public string ExecutionType { get; set; } = "";
        public bool? DisplayOutOfSeason { get; set; }
    }
}