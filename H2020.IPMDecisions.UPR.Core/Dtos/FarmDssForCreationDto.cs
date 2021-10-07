using System;
using System.ComponentModel.DataAnnotations;

namespace H2020.IPMDecisions.UPR.Core.Dtos
{
    public class FarmDssForCreationDto : DssForCreationDto
    {
        public Guid? FieldId { get; set; }
        public string FieldName { get; set; }
        public virtual DateTime? SowingDate { get; set; }
        [Required(ErrorMessage = "Crop EPPO Code is required")]
        [MaxLength(6, ErrorMessage = "EPPO Codes max length is 6 characters")]
        public string CropEppoCode { get; set; }

        [Required(ErrorMessage = "Pest EPPO Code is required")]
        [MaxLength(6, ErrorMessage = "EPPO Codes max length is 6 characters")]
        public string PestEppoCode { get; set; }
        [Required]
        public override string DssId { get; set; }
        [Required]
        public override string DssName { get; set; }
        [Required]
        public override string DssModelId { get; set; }
        [Required]
        public override string DssModelName { get; set; }
        [Required]
        public override string DssVersion { get; set; }
        [Required]
        public override string DssModelVersion { get; set; }
        [Required]
        public override string DssExecutionType { get; set; }
        public override string DssParameters { get; set; }
        public override string DssEndPoint { get; set; }
    }
}