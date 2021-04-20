using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Schema;
using System;
using System.Collections.Generic;

namespace H2020.IPMDecisions.UPR.BLL.Helpers
{
    public static class JsonSchemaToJson
    {
        public static string ToJson(string jsonSchema)
        {
            try
            {
                var jsonObject = new JObject();
                JSchema schema = StringToJsonSchema(jsonSchema);
                ProcessJsonSchemaProperties(jsonObject, schema);
                return jsonObject.ToString();
            }
            catch (Exception ex)
            {
                return ex.Message.ToString();
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