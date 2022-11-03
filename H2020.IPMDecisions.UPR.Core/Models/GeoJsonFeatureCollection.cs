using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace H2020.IPMDecisions.UPR.Core.Models
{
    // This GeoJson only accepts Point Geometry, it is used only for the DSS/Location endpoint.
    // If it is going to use on other parts of the application, please remove hardcoded strings
    public class GeoJsonFeatureCollection
    {
        public GeoJsonFeatureCollection()
        {
            Type = "FeatureCollection";
            Features = new List<GeoJsonFeature>();
        }

        [JsonPropertyName("type")]
        public string Type { get; set; }
        [JsonPropertyName("features")]
        public List<GeoJsonFeature> Features { get; set; }
    }

    public class GeoJsonFeature
    {
        public GeoJsonFeature()
        {
            Type = "Feature";
            Properties = new GeoJsonProperties();
        }

        [JsonPropertyName("type")]
        public string Type { get; set; }
        [JsonPropertyName("properties")]
        public GeoJsonProperties Properties { get; set; }
        [JsonPropertyName("geometry")]
        public GeoJsonGeometry Geometry { get; set; }
    }
    public class GeoJsonGeometry
    {
        public GeoJsonGeometry()
        {
            Type = "Point";
            Coordinates = new List<double>();
        }

        [JsonPropertyName("type")]
        public string Type { get; set; }
        [JsonPropertyName("coordinates")]
        public List<double> Coordinates { get; set; }
    }
    public class GeoJsonProperties
    {
    }
}