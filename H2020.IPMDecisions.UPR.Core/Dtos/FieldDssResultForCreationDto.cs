using System.ComponentModel.DataAnnotations;

namespace H2020.IPMDecisions.UPR.Core.Dtos
{
    public class FieldDssResultForCreationDto
    {
        [Required]
        public string DssFullResult { get; set; }
    }
}