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

            if (entity.DssExecutionType.ToLower() != "link") return ValidationResult.Success;

            if (!string.IsNullOrEmpty(value.ToString()))
                return new ValidationResult(string.Format("The {0} field is required when a DSS type 'link' is selected.", validationContext.DisplayName));

            return ValidationResult.Success;
        }
    }
}