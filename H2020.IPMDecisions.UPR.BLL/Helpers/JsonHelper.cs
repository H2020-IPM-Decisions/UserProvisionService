using System;
using System.Linq;
using System.Text.RegularExpressions;
using Newtonsoft.Json.Linq;

namespace H2020.IPMDecisions.UPR.BLL.Helpers
{
    public static class JsonHelper
    {
        public static string CheckMissingJsonProperties(JProperty property)
        {
            foreach (var childrenProperty in property.Children())
            {
                var hasMissingValue = "";
                if (childrenProperty.Type.ToString().ToLower() == "object")
                {
                    hasMissingValue = CheckMissingObjectChildProperty(childrenProperty);
                }
                else
                {
                    hasMissingValue = CheckMissingOtherTypeChildProperty(childrenProperty);
                }
                if (!string.IsNullOrEmpty(hasMissingValue))
                    return hasMissingValue;
            }
            return "";
        }

        private static string CheckMissingOtherTypeChildProperty(JToken childrenProperty)
        {
            if (((JValue)childrenProperty).Value == null) return childrenProperty.Path;
            var value = ((JValue)childrenProperty).Value.ToString();
            if (string.IsNullOrEmpty(value))
                return childrenProperty.Path;
            return "";
        }

        private static string CheckMissingObjectChildProperty(JToken childrenProperty)
        {
            foreach (var property in childrenProperty.Children())
            {
                var propertyAsJProperty = (JProperty)property;
                if (propertyAsJProperty.Value == null) return property.Path;
                var value = propertyAsJProperty.Value.ToString();
                if (string.IsNullOrEmpty(value))
                    return property.Path;
            }
            return "";
        }

        public static string AddDefaultDatesToDssJsonInput(string jsonAsString)
        {
            // First search for {CURRENT_YEAR} and replace
            Regex currentYearRegex = new Regex(@"[{]+[\scurrent_year]+[}]+",
               RegexOptions.IgnoreCase | RegexOptions.IgnorePatternWhitespace);
            if (currentYearRegex.IsMatch(jsonAsString))
            {
                jsonAsString = currentYearRegex.Replace(jsonAsString, DateTime.Today.Year.ToString());
            }

            // Then check if any {CURRENT_YEAR +/- 1} exists and replace
            Regex currentYearWithExtraRegex = new Regex(@"[{]+[\scurrent_year+|\-\d]+[}]+",
               RegexOptions.IgnoreCase | RegexOptions.IgnorePatternWhitespace);
            if (currentYearWithExtraRegex.IsMatch(jsonAsString))
            {
                MatchCollection currentYearMatches = currentYearWithExtraRegex.Matches(jsonAsString);
                foreach (Match match in currentYearMatches)
                {
                    var valueAsString = match.Value.Replace("{", "").Replace("}", "");
                    string[] yearParts = Regex.Split(valueAsString, @"\+|\-");
                    var extraYears = yearParts.LastOrDefault();
                    var newYear = DateTime.Today.Year;
                    if (valueAsString.Contains("+"))
                    {
                        newYear += int.Parse(extraYears);
                    }
                    else
                    {
                        newYear -= int.Parse(extraYears);
                    }
                    jsonAsString = jsonAsString.Replace(match.Value, newYear.ToString());
                }
            }
            return jsonAsString;
        }
    }
}