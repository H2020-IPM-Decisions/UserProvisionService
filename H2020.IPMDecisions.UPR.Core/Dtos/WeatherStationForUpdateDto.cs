namespace H2020.IPMDecisions.UPR.Core.Dtos
{
    public class WeatherStationForUpdateDto
    {
        public string WeatherId { get; set; }
        public string Name { get; set; }
        public string Url { get; set; }
        public bool? IsForecast { get; set; }
        public bool? AuthenticationRequired { get; set; }
        public string WeatherStationId { get; set; }
    }
}