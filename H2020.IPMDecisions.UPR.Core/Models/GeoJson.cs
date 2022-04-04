using System.Collections.Generic;

namespace H2020.IPMDecisions.UPR.Core.Models
{
    public class GeoJson
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

            public string Type { get; set; }
            public List<GeoJsonFeature> Features { get; set; }
        }
        public class GeoJsonFeature
        {
            public GeoJsonFeature()
            {
                Type = "Feature";
            }

            public string Type { get; set; }
            public GeoJsonGeometry Geometry { get; set; }
            public GeoJsonProperties Properties { get; set; }
        }
        public class GeoJsonGeometry
        {
            public GeoJsonGeometry()
            {
                Type = "Point";
                Coordinates = new List<double>();
            }

            public string Type { get; set; }
            public List<double> Coordinates { get; set; }
        }
        public class GeoJsonProperties
        {
        }
    }
}