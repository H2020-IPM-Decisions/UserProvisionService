using System.ComponentModel.DataAnnotations;
using H2020.IPMDecisions.UPR.Core.Dtos;

namespace H2020.IPMDecisions.UPR.Core.Validations
{
    public class WeatherDataAuthRequiredAttribute : ValidationAttribute
    {
        public WeatherDataAuthRequiredAttribute()
        {
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            WeatherDataSourceForCreationDto entity = (WeatherDataSourceForCreationDto)validationContext.ObjectInstance;

            if (entity.AuthenticationRequired == false) return ValidationResult.Success;

            if (value == null)
                return new ValidationResult(string.Format("The {0} field is required when a weather data source with authentication access is required.", validationContext.DisplayName));

            return ValidationResult.Success;
        }
    }
}