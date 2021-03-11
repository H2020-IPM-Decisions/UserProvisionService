namespace H2020.IPMDecisions.UPR.Core.Dtos
{
    public class WeatherDataSourceForUpdateDto : WeatherDataSourceForManipulationDto
    {
        public override string Name { get ; set; }
        public override string Url { get; set; }
        public override bool? IsForecast { get; set; }
        public override bool? AuthenticationRequired { get; set; }
    }
}