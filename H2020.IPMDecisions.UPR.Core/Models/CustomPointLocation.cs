using Newtonsoft.Json;

namespace H2020.IPMDecisions.UPR.Core.Models
{
    public class CustomPointLocation
    {
        public double X { get; set; }
        public double Y { get; set; }
        public int SRID { get; set; }

        [JsonConstructor]
        public CustomPointLocation(double x, double y, int srid)
        {
            this.SRID = srid;
            this.Y = y;
            this.X = x;
        }
    }
}