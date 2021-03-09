using System.ComponentModel.DataAnnotations;

namespace H2020.IPMDecisions.UPR.Core.Dtos
{
    public class WeatherCredentials
    {
        [Required]
        public string Username { get; set; }
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}