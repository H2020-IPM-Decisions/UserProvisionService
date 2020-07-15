using NetTopologySuite.Geometries;
using Newtonsoft.Json;

namespace H2020.IPMDecisions.UPR.Core.Models
{
    public class CustomPointLocation : Point
    {
        [JsonConstructor]
        public CustomPointLocation(double x, double y, int srid) 
            : base(x, y)
        {
            SRID = srid;
        }
    }
}