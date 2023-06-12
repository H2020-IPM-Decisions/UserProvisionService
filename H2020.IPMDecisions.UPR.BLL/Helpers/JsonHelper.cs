using System.Linq;
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

        public static void RemoveProperty(JObject jsonAsObject, string propertyNameToRemove)
        {
            foreach (var property in jsonAsObject.Properties().ToList())
            {
                if (property.Name == propertyNameToRemove)
                {
                    property.Remove();
                }
                else if (property.Value is JObject nestedObj)
                {
                    RemoveProperty(nestedObj, propertyNameToRemove);
                }
                else if (property.Value is JArray array)
                {
                    foreach (var item in array)
                    {
                        if (item is JObject nestedArrayObj)
                        {
                            RemoveProperty(nestedArrayObj, propertyNameToRemove);
                        }
                    }
                }
            }
        }
    }
}