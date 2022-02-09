using System;
using System.Collections.Generic;
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

        public static DateTime ProcessWeatherDataPeriod(IEnumerable<WeatherDataPeriod> weatherDatePeriodList, JObject dssInputSchemaAsJson, int currentYear = -1)
        {
            foreach (var weatherDate in weatherDatePeriodList)
            {
                var weatherDateJson = weatherDate.Value.ToString();
                if (weatherDate.DeterminedBy.ToLower() == "input_schema_property")
                {
                    var token = dssInputSchemaAsJson.SelectTokens(weatherDateJson).FirstOrDefault();
                    if (token == null)
                        continue;

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
            // If we reach this point means that no valid weather data on schema
            var lastWeatherValue = weatherDatePeriodList.LastOrDefault();
            if (lastWeatherValue != null)
                throw new NullReferenceException(string.Format("{0} is not defined on the DSS parameters, please add parameter.",
                    weatherDatePeriodList.LastOrDefault().Value.ToString()));

            // ToDo Can this happen?
            throw new NullReferenceException("No valid parameters on schema to create weather data, please add parameters.");
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

        public static void AddUserDssParametersToDssInput(JObject userDssParametersJObject, JObject inputSchemaAsJObject)
        {
            var pathsList = CreateJsonPaths(userDssParametersJObject);
            foreach (var userParameterPath in pathsList)
            {
                var token = inputSchemaAsJObject.SelectToken(userParameterPath);
                var userToken = userDssParametersJObject.SelectToken(userParameterPath);
                if (token != null)
                {
                    token.Replace(userToken);
                    continue;
                }
                AddNewTokenToJObject(inputSchemaAsJObject, userParameterPath, userToken);
            }
        }

        public static List<string> CreateJsonPaths(JObject jsonObject)
        {
            List<string> pathsList = new List<string>();
            foreach (var jsonChild in jsonObject.Children())
            {
                CheckIfJTokenHasChildren(jsonChild, pathsList);
            }
            return pathsList;
        }

        private static void CheckIfJTokenHasChildren(JToken jsonChild, List<string> pathsList)
        {
            if (jsonChild.Children().Any())
            {
                foreach (var child in jsonChild.Children())
                {
                    CheckIfJTokenHasChildren(child, pathsList);
                }
            }
            else
            {
                pathsList.Add(jsonChild.Path);
            }
        }

        public static void AddNewTokenToJObject(JObject inputSchemaAsJson, string userParameterPath, JToken userToken)
        {
            // check if nested path
            var pathList = userParameterPath.Split(".");
            if (pathList.Count() == 1)
            {
                inputSchemaAsJson.Add(userParameterPath, userToken);
                return;
            }

            JToken existingToken = null;
            var nestedObject = new JObject();
            bool isLastPartOfPath = false;
            int pathIndex = 1;
            foreach (var nestedPropertyPath in pathList)
            {
                if (pathIndex == pathList.Count()) isLastPartOfPath = true;

                if (inputSchemaAsJson.ContainsKey(nestedPropertyPath))
                {
                    existingToken = inputSchemaAsJson.SelectToken(nestedPropertyPath);
                }
                else
                {
                    if (existingToken == null)
                    {
                        inputSchemaAsJson.Add(nestedPropertyPath, nestedObject);
                        existingToken = inputSchemaAsJson.SelectToken(nestedObject.Path);
                    }
                    else
                    {
                        if (isLastPartOfPath)
                        {
                            var newProperty = new JProperty(nestedPropertyPath, userToken);
                            if (existingToken.Children().Any())
                                existingToken.Children().FirstOrDefault().AddAfterSelf(newProperty);
                            else
                                nestedObject.Add(newProperty);
                        }
                        else
                        {
                            if (existingToken.SelectToken(nestedPropertyPath) != null)
                            {
                                existingToken = existingToken.SelectToken(nestedPropertyPath);
                            }
                            else
                            {
                                var childObject = new JObject();
                                nestedObject.Add(nestedPropertyPath, childObject);
                                existingToken = inputSchemaAsJson.SelectToken(childObject.Path);
                                nestedObject = childObject;
                            }
                        }
                    }
                }
                pathIndex++;
            }
        }
    }
}