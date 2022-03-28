using System;
using H2020.IPMDecisions.UPR.Core.Models;

namespace H2020.IPMDecisions.UPR.Core.Dtos
{
    public class FieldCropPestDssDto
    {
        public FieldCropPestDssDto()
        {
            this.DssTask = new DssTask();
        }

        public Guid Id { get; set; }
        public Guid CropPestDssId { get; set; }
        public CropPestDssDto CropPestDssDto { get; set; }
        public dynamic DssParameters { get; set; }
        public FieldCropPestDto FieldCropPest { get; set; }
        public FieldDssResultDto DssResult { get; set; }
        public DssTask DssTask { get; set; }
    }
}