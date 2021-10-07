using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace H2020.IPMDecisions.UPR.Core.Entities
{
    public class CropPestDss
    {
        [Key]
        public Guid Id { get; set; }
        public Guid CropPestId { get; set; }
        public CropPest CropPest { get; set; }
        [Required]
        public string DssId { get; set; }
        [Required]
        public string DssName { get; set; }
        [Required]
        public string DssModelId { get; set; }
        [Required]
        public string DssModelName { get; set; }
        [Required]
        public string DssVersion { get; set; }
        [Required]
        public string DssModelVersion { get; set; }
        [Required]
        public string DssExecutionType { get; set; }
        public string DssEndPoint { get; set; }

        public ICollection<FieldCropPestDss> FieldCropPestDsses { get; set; }
        public ICollection<CropPestDssResult> CropPestDssResults { get; set; }
    }
}