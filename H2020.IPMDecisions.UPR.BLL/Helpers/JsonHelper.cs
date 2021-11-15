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
            var value = ((JValue)childrenProperty).Value.ToString();
            if (string.IsNullOrEmpty(value))
                return childrenProperty.Path;
            return "";
        }

        private static string CheckMissingObjectChildProperty(JToken childrenProperty)
        {
            foreach (var property in childrenProperty.Children())
            {
                var value = ((JProperty)property).Value.ToString();
                if (string.IsNullOrEmpty(value))
                    return property.Path;
            }
            return "";
        }
    }
}