using System;
using System.ComponentModel.DataAnnotations;

namespace H2020.IPMDecisions.UPR.Core.Dtos
{
    public class CropPestDssForCreationDto : DssForCreationDto
    {
        [Required]
        public Guid FieldCropPestId { get; set; }
        [Required]
        public override string DssId { get; set; }
        [Required]
        public override string DssModelId { get; set; }
        [Required]
        public override string DssParameters { get; set; }
        [Required]
        public override string DssVersion { get; set; }
        [Required]
        public override string DssExecutionType { get; set; }
        public override string DssEndPoint { get; set; }
    }
}