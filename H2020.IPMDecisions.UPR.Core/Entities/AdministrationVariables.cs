using System.ComponentModel.DataAnnotations;
using H2020.IPMDecisions.UPR.Core.Enums;

namespace H2020.IPMDecisions.UPR.Core.Entities
{
    public class AdministrationVariable
    {
        [Key]
        public AdminValuesEnum Id { get; set; }
        public string Description { get; set; }
        public string Value { get; set; }
    }
}