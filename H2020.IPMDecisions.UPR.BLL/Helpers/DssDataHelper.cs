using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using H2020.IPMDecisions.UPR.Core.Entities;
using H2020.IPMDecisions.UPR.Core.Models;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Schema;

namespace H2020.IPMDecisions.UPR.BLL.Helpers
{
    public static class DssDataHelper
    {
        public static string AddDefaultDatesToDssJsonInput(string jsonAsString, int? currentYear = -1, bool isHistorical = false)
        {
            if (currentYear == -1) currentYear = DateTime.Today.Year;
            if (isHistorical) currentYear = currentYear - 1;

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
            if (dssInputInformation.Input != null && dssInputInformation.Input.WeatherDataPeriodEnd != null)
            {
                var weatherEndDate = ProcessWeatherDataPeriod(dssInputInformation.Input.WeatherDataPeriodEnd, dssInputSchemaAsJson);
                currentYear = weatherEndDate.Year;

                // Get start date, and check todays date if middle season
                if (DateTime.Today > weatherEndDate)
                {
                    bool hasNewSeasonStarted = DatesInMiddleSeason(dssInputInformation.Input, currentYear, dssInputSchemaAsJson);
                    if (hasNewSeasonStarted) currentYear += 1;
                }
            }
            return currentYear;
        }

        private static bool DatesInMiddleSeason(DssModelSchemaInput input, int currentYear, JObject dssInputSchemaAsJson)
        {
            if (input.WeatherDataPeriodStart != null)
            {
                var weatherStartDate = ProcessWeatherDataPeriod(input.WeatherDataPeriodStart, dssInputSchemaAsJson, currentYear);
                var startDateYear = weatherStartDate.Year;
                if ((startDateYear == currentYear && weatherStartDate < DateTime.Today) || startDateYear < currentYear) return true;
            }
            return false;
        }

        public static DateTime ProcessWeatherDataPeriod(IEnumerable<WeatherDataPeriod> weatherDatePeriodList, JObject dssInputSchemaAsJson, int currentYear = -1, bool isHistorical = false)
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
                    if (!DateTime.TryParse(dateString, CultureInfo.InvariantCulture, DateTimeStyles.None, out dateValue))
                        dateValue = DateTime.Parse(DssDataHelper.AddDefaultDatesToDssJsonInput(dateString));

                    // This exception is for the SEPTORIAHU model
                    if (token.Path.ToLower().Contains("dategs31")) return dateValue.AddDays(-1);
                    return dateValue;
                }
                else // "fixed_date" as specified on /dss/rest/schema/dss
                {
                    return DateTime.Parse(DssDataHelper.AddDefaultDatesToDssJsonInput(weatherDateJson, currentYear, isHistorical));
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
            // Check if has any value with a default, if so, do not delete
            foreach (var property in notRequiredProperties)
            {
                var selectedProperty = schema.Properties.Where(p => p.Key == property).FirstOrDefault().Value;
                // This shouldn't happen... but just in case
                if (selectedProperty == null)
                {
                    continue;
                }
                var selectedPropertyValues = RemovePropertiesWithoutDefaultValueOnJSchema(selectedProperty);
                if (selectedPropertyValues == null)
                {
                    schema.Properties.Remove(property);
                }
            }
        }

        private static JSchema RemovePropertiesWithoutDefaultValueOnJSchema(JSchema schema)
        {
            // This shouldn't happen... but just in case
            if (schema == null)
            {
                return schema;
            }
            if (schema.Type.ToString().ToLower() == "object")
            {
                var propertiesWithoutDefault = schema.Properties.Where(p => p.Value.Default == null);
                foreach (var item in propertiesWithoutDefault)
                {
                    schema.Properties.Remove(item.Key);
                }
            }
            else if (schema.Default == null)
            {
                schema = null;
            }
            return schema;
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
                var matchObject = Regex.Match(userParameterPath, @"\w+[[]\d+[]].");
                var isAnArrayObject = (matchObject.Success) ? true : false;
                if (isAnArrayObject)
                {
                    var startIndex = userParameterPath.IndexOf("[");
                    var parentArrayParameterPath = userParameterPath.Substring(0, startIndex);
                    var arrayToken = userDssParametersJObject.SelectToken(parentArrayParameterPath);
                    AddNewArrayTokenToJObject(inputSchemaAsJObject, parentArrayParameterPath, arrayToken);
                }
                else
                {
                    AddNewTokenToJObject(inputSchemaAsJObject, userParameterPath, userToken);
                }
            }
        }

        private static void AddNewArrayTokenToJObject(JObject inputSchemaAsJObject, string arrayPath, JToken arrayToken)
        {
            var tokenExists = inputSchemaAsJObject.SelectToken(arrayPath);
            if (tokenExists != null)
            {
                tokenExists.Replace(arrayToken);
            }
            else
            {
                AddNewTokenToJObject(inputSchemaAsJObject, arrayPath, arrayToken);
            }
        }

        private static string UpdateTokenValue(string jsonString, string tokenName, string newValue)
        {
            JObject jsonObj = (JObject)Newtonsoft.Json.JsonConvert.DeserializeObject(jsonString);
            bool updated = UpdateTokenRecursive(jsonObj, tokenName, newValue);

            if (updated)
            {
                return jsonObj.ToString();
            }
            else
            {
                Console.WriteLine($"{tokenName} not found.");
                return jsonString; // Return the original JSON if the token was not found
            }
        }

        public static string UpdateDssParametersToNewCalendarYear(IEnumerable<WeatherDataPeriod> weatherDatePeriodList, string dssParameters)
        {
            try
            {
                foreach (var weatherDate in weatherDatePeriodList)
                {
                    var weatherDateJson = weatherDate.Value.ToString();
                    if (weatherDate.DeterminedBy.ToLower() == "input_schema_property")
                    {
                        JObject dssInputSchemaAsJson = JObject.Parse(dssParameters.ToString());
                        var token = dssInputSchemaAsJson.SelectTokens(weatherDateJson).FirstOrDefault();
                        if (token == null)
                            continue;
                        string dateString = token.ToString();
                        DateTime dateValue;
                        if (!DateTime.TryParse(dateString, CultureInfo.InvariantCulture, DateTimeStyles.None, out dateValue))
                            dateValue = DateTime.Parse(DssDataHelper.AddDefaultDatesToDssJsonInput(dateString));

                        var newDateValue = dateValue.AddYears(1).ToString("yyyy-MM-dd");
                        return UpdateTokenValue(dssParameters, weatherDateJson, newDateValue.ToString());
                    }
                }
                return dssParameters;
            }
            catch (Exception ex)
            {
                throw new Exception("Error UpdateDssParametersToNewCalendarYear", ex);
            }
        }

        public static JObject AddFieldObservation(JObject obj, DssModelFieldObservation fieldObservation)
        {
            foreach (var property in obj.Properties().ToList())
            {
                if (property.Name == "fieldObservations")
                {
                    // Add the fieldObservation to the fieldObservations array
                    var fieldObservations = (JArray)property.Value;
                    int numberOfFieldObservations = fieldObservations.Count;
                    for (int i = 0; i < numberOfFieldObservations; i++)
                    {
                        var item = (JObject)fieldObservations[i];
                        item.Add("fieldObservation", JObject.FromObject(fieldObservation));
                    }
                }
                else if (property.Value is JObject nestedObj)
                {
                    // Recursively search for the fieldObservations property
                    AddFieldObservation(nestedObj, fieldObservation);
                }
                else if (property.Value is JArray array)
                {
                    foreach (var item in array)
                    {
                        if (item is JObject nestedArrayObj)
                        {
                            // Recursively search for the fieldObservations property
                            AddFieldObservation(nestedArrayObj, fieldObservation);
                        }
                    }
                }
            }
            return obj;
        }

        public static JObject UpdateFieldObservationWithNoTimeProperty(JObject obj, DssModelFieldObservationNoTime fieldObservation)
        {
            foreach (var property in obj.Properties().ToList())
            {
                if (property.Name == "fieldObservations")
                {
                    // Add the fieldObservation to the fieldObservations array
                    var fieldObservations = (JArray)property.Value;
                    int numberOfFieldObservations = fieldObservations.Count;
                    for (int i = 0; i < numberOfFieldObservations; i++)
                    {
                        var item = (JObject)fieldObservations[i]["fieldObservation"];
                        JObject observationNoTimeObject = JObject.FromObject(fieldObservation);

                        item.Merge(observationNoTimeObject, new JsonMergeSettings
                        {
                            MergeArrayHandling = MergeArrayHandling.Replace,
                            MergeNullValueHandling = MergeNullValueHandling.Merge
                        });
                    }
                }
                else if (property.Value is JObject nestedObj)
                {
                    // Recursively search for the fieldObservations property
                    UpdateFieldObservationWithNoTimeProperty(nestedObj, fieldObservation);
                }
                else if (property.Value is JArray array)
                {
                    foreach (var item in array)
                    {
                        if (item is JObject nestedArrayObj)
                        {
                            // Recursively search for the fieldObservations property
                            UpdateFieldObservationWithNoTimeProperty(nestedArrayObj, fieldObservation);
                        }
                    }
                }
            }
            return obj;
        }

        private static List<string> CreateJsonPaths(JObject jsonObject)
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

        private static void AddNewTokenToJObject(JObject inputSchemaAsJson, string userParameterPath, JToken userToken)
        {
            // check if it is an array
            var match = Regex.Match(userParameterPath, @"\w+[[]\d+[]]");
            var isAnArray = (match.Success) ? true : false;
            if (isAnArray)
            {
                userParameterPath = userParameterPath.Substring(0, userParameterPath.IndexOf("["));
            }
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

                if (inputSchemaAsJson.ContainsKey(nestedPropertyPath) & existingToken == null)
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
                            if (isAnArray)
                            {
                                var arrayExists = existingToken.SelectToken(nestedPropertyPath);
                                if (arrayExists != null)
                                {
                                    arrayExists.Children().LastOrDefault().AddAfterSelf(userToken);
                                    continue;
                                }
                                JArray arrayObject = new JArray();
                                arrayObject.Add(userToken);
                                newProperty = new JProperty(nestedPropertyPath, arrayObject);
                            }
                            if (existingToken.Children().Any())
                            {
                                var propertyExits = existingToken.SelectToken(nestedPropertyPath);
                                if (propertyExits == null)
                                {
                                    existingToken.Children().FirstOrDefault().AddAfterSelf(newProperty);
                                    continue;
                                }
                            }
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

        public static JObject AddDefaultDssParametersToInputSchema(JObject inputAsJsonObject, JObject userParametersAsJsonObject)
        {
            var pathsListDssInputSchema = DssDataHelper.CreateJsonPaths(inputAsJsonObject);
            var pathsListDssParameters = DssDataHelper.CreateJsonPaths(userParametersAsJsonObject);
            var arrayNotProcessed = true;
            foreach (var userParameterPath in pathsListDssParameters)
            {
                var userParameterPathForModification = userParameterPath;
                var match = Regex.Match(userParameterPathForModification, @"\w+[[]\d+[]]");
                var isAnArray = (match.Success) ? true : false;
                if (isAnArray) userParameterPathForModification = userParameterPathForModification.Substring(0, userParameterPathForModification.IndexOf("["));
                var tokenFromDssParameter = userParametersAsJsonObject.SelectToken(userParameterPathForModification);
                var pathPropertyName = userParameterPathForModification.Split(".").LastOrDefault();

                var tokensPathFromInput = pathsListDssInputSchema.Where(s => s.Contains(pathPropertyName));
                if (tokensPathFromInput.Count() == 0)
                {
                    // Is not part of the UI schema, do not add value
                    continue;
                }
                var hasDefaultProperty = tokensPathFromInput.Any(s => s.Contains("default"));
                if (!hasDefaultProperty || (isAnArray & hasDefaultProperty & arrayNotProcessed))
                {
                    var firstPartOfTheTokenList = tokensPathFromInput.FirstOrDefault();
                    var stringToReplace = firstPartOfTheTokenList.Split(".").LastOrDefault();
                    var newDefaultPath = firstPartOfTheTokenList.Replace(stringToReplace, "default");

                    AddNewTokenToJObject(inputAsJsonObject, newDefaultPath, tokenFromDssParameter);
                    pathsListDssInputSchema.Insert(0, newDefaultPath);
                    if (isAnArray) arrayNotProcessed = false;
                }

                var defaultPropertyPath = pathsListDssInputSchema.Where(s => s.Contains(pathPropertyName) & s.Contains("default")).FirstOrDefault();
                if (defaultPropertyPath == null) continue;

                var token = inputAsJsonObject.SelectToken(defaultPropertyPath);
                if (token != null)
                {
                    token.Replace(tokenFromDssParameter);
                    continue;
                }
            }
            return inputAsJsonObject;
        }

        public static JObject HideInternalDssParametersFromInputSchema(JObject inputAsJsonObject, List<string> dssInternalParameters)
        {
            var pathsListDssInputSchema = DssDataHelper.CreateJsonPaths(inputAsJsonObject);

            var optionsPropertyName = "options";
            var hiddenPropertyName = "hidden";
            var hiddenPropertyValue = true;
            foreach (var internalParameterPath in dssInternalParameters)
            {
                var pathPropertyName = internalParameterPath.Split(".").LastOrDefault();
                var tokensPathFromInput = pathsListDssInputSchema.Where(s => s.Contains(pathPropertyName));

                var hasOptionsProperty = tokensPathFromInput.Any(s => s.Contains(optionsPropertyName));
                if (!hasOptionsProperty)
                {
                    var firstPartOfTheTokenList = tokensPathFromInput.FirstOrDefault();
                    var stringToReplace = firstPartOfTheTokenList.Split(".").LastOrDefault();
                    var newOptionsPath = firstPartOfTheTokenList.Replace(stringToReplace, optionsPropertyName);
                    var childObject = new JObject(){
                        new JProperty(hiddenPropertyName, hiddenPropertyValue)
                    };
                    AddNewTokenToJObject(inputAsJsonObject, newOptionsPath, childObject);
                }
            }
            return inputAsJsonObject;
        }

        public static string PrepareDssParametersForHistoricalYear(JObject dssInputSchema, string dssParameters, int yearsToRemove = 1)
        {
            try
            {
                List<string> dateProperties = new List<string>();
                FindDateProperties(dssInputSchema, dateProperties);
                dssParameters = RemoveNYearsDatesDssParameters(dateProperties, dssParameters, yearsToRemove);
                return dssParameters;
            }
            catch (Exception ex)
            {
                throw new Exception("Error PrepareDssParametersForHistoricalYear", ex);
            }
        }

        private static string RemoveNYearsDatesDssParameters(List<string> propertiesToModify, string dssParameters, int yearsToRemove = 1)
        {
            try
            {
                JObject dssInputSchemaAsJson = JObject.Parse(dssParameters.ToString());
                foreach (var property in propertiesToModify)
                {
                    var token = FindProperty(dssInputSchemaAsJson, property);
                    if (token == null)
                        continue;
                    string dateString = token.ToString();
                    DateTime dateValue;
                    if (string.IsNullOrEmpty(dateString))
                        continue;
                    if (!DateTime.TryParse(dateString, CultureInfo.InvariantCulture, DateTimeStyles.None, out dateValue))
                        dateValue = DateTime.Parse(DssDataHelper.AddDefaultDatesToDssJsonInput(dateString));
                    var newDateValue = dateValue.AddYears(-yearsToRemove).ToString("yyyy-MM-dd");
                    dssParameters = UpdateTokenValue(dssParameters, property, newDateValue.ToString());
                }
                return dssParameters;
            }
            catch (Exception ex)
            {
                throw new Exception("Error RemoveNYearsDatesDssParameters", ex);
            }
        }

        public static Dictionary<string, string> ConvertJTokenValuesToString(Dictionary<string, JToken> jsonDict)
        {
            var stringDict = new Dictionary<string, string>();
            foreach (var kvp in jsonDict)
            {
                stringDict[kvp.Key] = kvp.Value.ToString();
            }
            return stringDict;
        }

        public static string BuildQueryString(Dictionary<string, string> parameters)
        {
            var queryParameters = new List<string>();
            foreach (var kvp in parameters)
            {
                queryParameters.Add($"{Uri.EscapeDataString(kvp.Key)}={Uri.EscapeDataString(kvp.Value)}");
            }
            return string.Join("&", queryParameters);
        }

        public static JObject UpdateSystemParameters(JObject dssParametersAsJsonObject, CropPest cropPest)
        {
            UpdateProperty(dssParametersAsJsonObject, "crop", cropPest.CropEppoCode);
            UpdateProperty(dssParametersAsJsonObject, "pest", cropPest.PestEppoCode);
            return dssParametersAsJsonObject;
        }

        static void UpdateProperty(JToken token, string propertyName, string newValue)
        {
            if (token.Type == JTokenType.Object)
            {
                var obj = (JObject)token;
                foreach (var property in obj.Properties())
                {
                    if (string.Equals(property.Name, propertyName, StringComparison.OrdinalIgnoreCase))
                    {
                        obj[property.Name] = newValue;
                    }
                    UpdateProperty(property.Value, propertyName, newValue);
                }
            }
            else if (token.Type == JTokenType.Array)
            {
                foreach (var item in (JArray)token)
                {
                    UpdateProperty(item, propertyName, newValue);
                }
            }
        }

        private static void FindDateProperties(JToken token, List<string> dateProperties)
        {
            if (token.Type == JTokenType.Object)
            {
                foreach (var property in (JObject)token)
                {
                    if (property.Value.Type == JTokenType.Object)
                    {
                        var obj = (JObject)property.Value;
                        var formatToken = obj["format"];
                        if (formatToken != null && formatToken.ToString() == "date")
                        {
                            dateProperties.Add(property.Key);
                        }
                        FindDateProperties(property.Value, dateProperties);
                    }
                    else if (property.Value.Type == JTokenType.Array)
                    {
                        foreach (var item in (JArray)property.Value)
                        {
                            FindDateProperties(item, dateProperties);
                        }
                    }
                }
            }
        }

        private static JToken FindProperty(JToken token, string propertyName)
        {
            if (token.Type == JTokenType.Object)
            {
                foreach (var property in (JObject)token)
                {
                    if (property.Key == propertyName)
                    {
                        return property.Value;
                    }
                    JToken result = FindProperty(property.Value, propertyName);
                    if (result != null)
                    {
                        return result;
                    }
                }
            }
            else if (token.Type == JTokenType.Array)
            {
                foreach (var item in (JArray)token)
                {
                    JToken result = FindProperty(item, propertyName);
                    if (result != null)
                    {
                        return result;
                    }
                }
            }
            return null;
        }

        private static bool UpdateTokenRecursive(JToken token, string tokenName, string newValue)
        {
            if (token.Type == JTokenType.Object)
            {
                JObject obj = (JObject)token;
                foreach (var property in obj.Properties())
                {
                    if (property.Name == tokenName)
                    {
                        property.Value = newValue;
                        return true;
                    }
                    if (UpdateTokenRecursive(property.Value, tokenName, newValue))
                    {
                        return true;
                    }
                }
            }
            else if (token.Type == JTokenType.Array)
            {
                foreach (var item in (JArray)token)
                {
                    if (UpdateTokenRecursive(item, tokenName, newValue))
                    {
                        return true;
                    }
                }
            }
            return false;
        }
    }
}