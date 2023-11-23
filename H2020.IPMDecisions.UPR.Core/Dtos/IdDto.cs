using System;
using System.ComponentModel.DataAnnotations;

namespace H2020.IPMDecisions.UPR.Core.Dtos
{
    public class IdDto
    {
        [Required]
        public Guid Id { get; set; }
    }
}