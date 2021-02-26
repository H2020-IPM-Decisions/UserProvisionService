using System.ComponentModel.DataAnnotations;

namespace H2020.IPMDecisions.UPR.Core.Dtos
{
    public class WeatherDataSourceDto
    {
        [Required]
        public string Id { get; set; }
        public string Name { get; set; }
        public string Url { get; set; }
    }
}