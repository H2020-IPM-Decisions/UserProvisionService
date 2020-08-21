using System;

namespace H2020.IPMDecisions.UPR.Core.Dtos
{
    public class CropPestDto
    {
        public Guid Id { get; set; }
        public string CropEppoCode { get; set; }
        public string PestEppoCode { get; set; }
    }
}