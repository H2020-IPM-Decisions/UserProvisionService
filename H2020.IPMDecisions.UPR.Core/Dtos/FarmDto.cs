using System;
using NetTopologySuite.Geometries;

namespace H2020.IPMDecisions.UPR.Core.Dtos
{
    public class FarmDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Inf1 { get; set; }
        public string Inf2 { get; set; }
        public Point Location { get; set; }
    }
}