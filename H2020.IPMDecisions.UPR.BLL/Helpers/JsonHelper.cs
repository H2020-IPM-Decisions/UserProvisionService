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

        public static bool HasProperty<T>(T obj, string propertyName) where T : JToken
        {
            if (obj == null || string.IsNullOrEmpty(propertyName))
                return false;

            if (obj is JObject jObject)
            {
                if (jObject.ContainsKey(propertyName))
                    return true;

                foreach (var property in jObject.Properties())
                {
                    if (property.Value is JToken valueToken && HasProperty(valueToken, propertyName))
                        return true;
                }
            }
            else if (obj is JArray jArray)
            {
                foreach (var item in jArray)
                {
                    if (HasProperty(item, propertyName))
                        return true;
                }
            }
            return false;
        }

        public static string GetPropertyPath(JToken token, string propertyName)
        {
            if (token.Type == JTokenType.Object)
            {
                JObject obj = (JObject)token;
                foreach (var property in obj.Properties())
                {
                    if (property.Name == propertyName)
                        return property.Path;

                    string path = GetPropertyPath(property.Value, propertyName);
                    if (!string.IsNullOrEmpty(path))
                        return path;
                }
            }
            else if (token.Type == JTokenType.Array)
            {
                JArray array = (JArray)token;
                foreach (var item in array)
                {
                    string path = GetPropertyPath(item, propertyName);
                    if (!string.IsNullOrEmpty(path))
                        return path;
                }
            }

            return null;
        }

        public static JProperty FindPropertyByName(JToken token, string propertyName, string excludedProperty = "", string currentPath = "")
        {
            if (token is JObject obj)
            {
                foreach (var property in obj.Properties())
                {
                    string propertyPath = currentPath + "." + property.Name;
                    if (property.Name == excludedProperty)
                    {
                        continue;
                    }

                    if (property.Name == propertyName)
                    {
                        return property;
                    }
                    else
                    {
                        JProperty childProperty = FindPropertyByName(property.Value, propertyName, excludedProperty, propertyPath);
                        if (childProperty != null)
                        {
                            return childProperty;
                        }
                    }
                }
            }
            else if (token is JArray array)
            {
                for (int i = 0; i < array.Count; i++)
                {
                    string arrayPath = currentPath + "[" + i + "]";
                    if (arrayPath == excludedProperty)
                    {
                        continue;
                    }

                    JProperty childProperty = FindPropertyByName(array[i], propertyName, excludedProperty, arrayPath);
                    if (childProperty != null)
                    {
                        return childProperty;
                    }
                }
            }
            return null;
        }
    }
}