using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Schema;
using System;
using System.Collections.Generic;

namespace H2020.IPMDecisions.UPR.BLL.Helpers
{
    public static class JsonSchemaToJson
    {
        public static string ToJsonString(string jsonSchema, ILogger logger)
        {
            try
            {
                var jsonObject = ToJsonObject(jsonSchema, logger);
                return jsonObject.ToString();
            }
            catch (Exception ex)
            {
                logger.LogError(string.Format("Error converting ToJsonString. {0}", ex.Message));
                return ex.Message.ToString();
            }
        }

        public static JObject ToJsonObject(string jsonSchema, ILogger logger)
        {
            try
            {
                var jsonObject = new JObject();
                JSchema schema = StringToJsonSchema(jsonSchema);
                return ProcessJsonSchemaProperties(jsonObject, schema);
            }
            catch (Exception ex)
            {
                logger.LogError(string.Format("Error converting ToJsonObject. {0}", ex.Message));
                return null;
            }
        }

        private static JObject ProcessJsonSchemaProperties(JObject json, JSchema schema)
        {
            foreach (var property in schema.Properties)
            {
                switch (property.Value.Type.ToString().ToLower())
                {
                    case "integer":
                    case "number":
                    case "boolean":
                    case "string":
                        json.Add(ProcessStandardTypeProperty(property));
                        break;
                    case "array":
                        json.Add(property.Key.ToString(), ProcessArrayProperty(property));
                        break;
                    case "object":
                        json.Add(property.Key.ToString(), ProcessObjectProperty(property));
                        break;
                    default:
                        break;
                }
            }
            return json;
        }

        private static JSchema StringToJsonSchema(string jsonSchema)
        {
            JSchemaUrlResolver resolver = new JSchemaUrlResolver();
            return JSchema.Parse(jsonSchema, resolver);
        }

        private static JProperty ProcessStandardTypeProperty(KeyValuePair<string, JSchema> property)
        {
            if (HasDefaultValue(property.Value))
            {
                return new JProperty(property.Key, property.Value.Default.ToString());
            };
            return new JProperty(property.Key, null);
        }

        private static JObject ProcessObjectProperty(KeyValuePair<string, JSchema> property)
        {
            var jsonObject = new JObject();
            JSchema schema = StringToJsonSchema(property.Value.ToString());
            ProcessJsonSchemaProperties(jsonObject, schema);
            return jsonObject;
        }

        private static JObject ProcessArrayProperty(KeyValuePair<string, JSchema> property)
        {
            var jsonObject = new JObject();
            JSchema schema = StringToJsonSchema(property.Value.ToString());
            foreach (var item in schema.Items)
            {
                //ToDo: Do we need to do this or object saved on DB?
            }
            ProcessJsonSchemaProperties(jsonObject, schema);
            return jsonObject;
        }

        private static bool HasDefaultValue(JSchema propertyValue)
        {
            var defaultProperty = propertyValue.Default;
            if (defaultProperty != null) return true;
            return false;
        }
    }
}