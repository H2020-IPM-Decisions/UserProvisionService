using System;
using System.ComponentModel.DataAnnotations;

namespace H2020.IPMDecisions.UPR.Core.Dtos
{
    public class FarmDssForCreationDto : DssForCreationDto
    {
        public Guid? FieldId { get; set; }
        public string FieldName { get; set; }
        [Required]
        public CropPestForCreationDto CropPest { get; set; }
        [Required]
        public override string DssId { get; set; }
        [Required]
        public override string DssModelId { get; set; }
        [Required]
        public override string DssVersion { get; set; }
        [Required]
        public override string DssExecutionType { get; set; }
        [Required]
        public override string DssParameters { get; set; }
        public override string DssEndPoint { get; set; }
    }
}