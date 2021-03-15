using System.ComponentModel.DataAnnotations;
using H2020.IPMDecisions.UPR.Core.Dtos;

namespace H2020.IPMDecisions.UPR.Core.Validations
{
    public class NotWeatherForecastAttribute : ValidationAttribute
    {
        public NotWeatherForecastAttribute()
        {
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            WeatherDataSourceForManipulationDto entity = (WeatherDataSourceForManipulationDto)validationContext.ObjectInstance;

            if (entity.IsForecast == true || entity.IsForecast == null) return ValidationResult.Success;

            if (value == null)
                return new ValidationResult(string.Format("The {0} field is required when a no forecast weather data source is selected.", validationContext.DisplayName));

            return ValidationResult.Success;
        }
    }
}