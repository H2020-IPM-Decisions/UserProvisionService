using System.ComponentModel.DataAnnotations;
using H2020.IPMDecisions.UPR.Core.Dtos;

namespace H2020.IPMDecisions.UPR.Core.Validations
{
    public class DssEndPointRequiredAttribute : ValidationAttribute
    {
        public DssEndPointRequiredAttribute()
        {
        }
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            DssForCreationDto entity = (DssForCreationDto)validationContext.ObjectInstance;

            var dssExecutionType = "link";

            if (entity.DssExecutionType.ToLower() != dssExecutionType) return ValidationResult.Success;

            if (value == null || string.IsNullOrEmpty(value.ToString()))
                return new ValidationResult(string.Format("The {0} field is required when a DSS with Execution Type '{1}' is selected.", validationContext.DisplayName, dssExecutionType.ToUpper()));

            return ValidationResult.Success;
        }
    }
}