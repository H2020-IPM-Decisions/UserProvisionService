using System;
using System.Linq;
using System.Text.RegularExpressions;
using H2020.IPMDecisions.UPR.Core.Models;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Schema;

namespace H2020.IPMDecisions.UPR.BLL.Helpers
{
    public static class DssDataHelper
    {
        public static string AddDefaultDatesToDssJsonInput(string jsonAsString, int? currentYear = -1)
        {
            if (currentYear == -1) currentYear = DateTime.Today.Year;

            Regex currentYearRegex = new Regex(@"[{]+[\scurrent_year]+[}]+",
               RegexOptions.IgnoreCase | RegexOptions.IgnorePatternWhitespace);
            if (currentYearRegex.IsMatch(jsonAsString))
            {
                jsonAsString = currentYearRegex.Replace(jsonAsString, currentYear.ToString());
            }
            Regex previousYearRegex = new Regex(@"[{]+[\sprevious_year]+[}]+",
                RegexOptions.IgnoreCase | RegexOptions.IgnorePatternWhitespace);
            if (previousYearRegex.IsMatch(jsonAsString))
            {
                jsonAsString = previousYearRegex.Replace(jsonAsString, (currentYear - 1).ToString());
            }
            return jsonAsString;
        }

        public static int GetCurrentYearForDssDefaultDates(DssModelInformation dssInputInformation, JObject dssInputSchemaAsJson)
        {
            var currentYear = -1;
            if (dssInputInformation.Input.WeatherDataPeriodEnd != null)
            {
                var weatherEndDate = ProcessWeatherDataPeriod(dssInputInformation.Input.WeatherDataPeriodEnd, dssInputSchemaAsJson);
                currentYear = weatherEndDate.Year;
                if (DateTime.Today > weatherEndDate) currentYear += 1;
            }
            return currentYear;
        }

        public static DateTime ProcessWeatherDataPeriod(WeatherDataPeriod weatherDataPeriod, JObject dssInputSchemaAsJson, int currentYear = -1)
        {
            var weatherDateJson = weatherDataPeriod.Value.ToString();
            if (weatherDataPeriod.DeterminedBy.ToLower() == "input_schema_property")
            {
                var token = dssInputSchemaAsJson.SelectTokens(weatherDateJson).FirstOrDefault();
                if (token == null)
                    throw new NullReferenceException(string.Format("{0} is not defined on the DSS parameters, please add parameter.", weatherDateJson));

                string dateString = token.ToString();
                DateTime dateValue; 
                if (DateTime.TryParse(dateString, out dateValue))
                    return dateValue;
                else
                    return DateTime.Parse(DssDataHelper.AddDefaultDatesToDssJsonInput(dateString));
            }
            else // "fixed_date" as specified on //dss/rest/schema/dss
            {
                return DateTime.Parse(DssDataHelper.AddDefaultDatesToDssJsonInput(weatherDateJson, currentYear));
            }
        }

        public static void RemoveNotRequiredInputSchemaProperties(JSchema inputSchema)
        {
            RemoveNotRequiredOnJSchema(inputSchema);
            foreach (var schemaProperty in inputSchema.Properties.Values)
            {
                RemoveNotRequiredOnJSchema(schemaProperty);
            }
        }

        private static void RemoveNotRequiredOnJSchema(JSchema schema)
        {
            // Always remove weather data as the code gets the data later
            var weatherDataKey = schema.Properties.Keys.Where(k => k.ToLower() == "weatherdata").FirstOrDefault();
            if (!string.IsNullOrEmpty(weatherDataKey))
            {
                schema.Properties.Remove(weatherDataKey);
                schema.Required.Remove(weatherDataKey);
            }

            var notRequiredProperties = schema.Properties.Keys.Where(k => !schema.Required.Any(k2 => k2 == k));
            foreach (var property in notRequiredProperties)
            {
                schema.Properties.Remove(property);
            }
        }
    }
}