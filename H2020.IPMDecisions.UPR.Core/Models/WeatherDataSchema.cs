namespace H2020.IPMDecisions.UPR.Core.Models
{
    // This class matches the schema definition of /api/wx/rest/schema/weatherdata
    public class WeatherDataSchema
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string EndPoint { get; set; }
    }
}