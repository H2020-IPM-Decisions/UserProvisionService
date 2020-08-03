using System.ComponentModel.DataAnnotations;

namespace H2020.IPMDecisions.UPR.Core.Dtos
{
    public class DataShareRequestDto
    {
        [Required]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }
    }
}