using H2020.IPMDecisions.UPR.Core.Enums;

namespace H2020.IPMDecisions.UPR.Core.Models
{
    public class WeatherDataResult
    {
        public bool Continue { get; set; }
        public string ResponseWeatherAsString { get; set; }
        public WeatherDataResponseSchema ResponseWeather { get; set; }
        public DssOutputMessageTypeEnum ErrorType { get; set; }
    }
}