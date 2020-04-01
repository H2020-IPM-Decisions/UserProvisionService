using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Reflection;

namespace H2020.IPMDecisions.UPR.Core.Helpers
{
    public static class ObjectExtensions
    {
        public static ExpandoObject ShapeData<TSource>(
            this TSource source,
            string fields = "")
        {
            if (source is null)
            {
                throw new System.ArgumentNullException(nameof(source));
            }

            var dataShapedObject = new ExpandoObject();
            if (string.IsNullOrWhiteSpace(fields))
            {
                var propertyInfos = typeof(TSource)
                    .GetProperties(BindingFlags.Public | BindingFlags.Instance);

                foreach (var propertyInfo in propertyInfos)
                {
                    var propertyValue = propertyInfo.GetValue(source);
                    ((IDictionary<string, object>)dataShapedObject)
                        .Add(propertyInfo.Name, propertyValue);
                }

                return dataShapedObject;
            }
            else
            {
                var fieldsAfterSplit = fields.Split(",");

                foreach (var field in fieldsAfterSplit)
                {
                    var propertyName = field.Trim();
                    var propertyInfo = typeof(TSource)
                        .GetProperty(propertyName, BindingFlags.IgnoreCase |
                            BindingFlags.Public | BindingFlags.Instance);

                    if (propertyInfo == null)
                        throw new Exception($"Property {propertyName} wasn't found on {typeof(TSource)}");

                    var propertyValue = propertyInfo.GetValue(source);
                    ((IDictionary<string, object>)dataShapedObject)
                        .Add(propertyInfo.Name, propertyValue);
                }
                return dataShapedObject;
            }
        }
    }
}