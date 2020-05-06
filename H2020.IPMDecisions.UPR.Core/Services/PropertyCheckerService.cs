using System.Reflection;

namespace H2020.IPMDecisions.UPR.Core.Services
{
    public class PropertyCheckerService : IPropertyCheckerService
    {
        public bool TypeHasProperties<T>(string fields, bool mustHaveIdField = false)
        {
            if (string.IsNullOrWhiteSpace(fields))
            {
                return true;
            }

            var fieldsAfterSplit = fields.Split(',');
            var hasIdField = false;
            foreach (var field in fieldsAfterSplit)
            {
                var propertyName = field.Trim();
                if (propertyName.ToLower() == "id") hasIdField = true;

                var propertyInfo = typeof(T)
                    .GetProperty(propertyName, BindingFlags.IgnoreCase |
                        BindingFlags.Public | BindingFlags.Instance);
                
                if (propertyInfo == null)
                    return false;
            }
            if(mustHaveIdField && !hasIdField) return false;

            return true;
        }
    }
}