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
                JSchema schema = StringToJsonSchema(jsonSchema, logger);
                return ProcessJsonSchemaProperties(jsonObject, schema, logger);
            }
            catch (Exception ex)
            {
                logger.LogError(string.Format("Error converting ToJsonObject. {0}", ex.Message));
                return null;
            }
        }

        private static JObject ProcessJsonSchemaProperties(JObject json, JSchema schema, ILogger logger)
        {
            foreach (var property in schema.Properties)
            {
                switch (property.Value.Type.ToString().ToLower())
                {
                    case "integer":
                    case "boolean":
                        json.Add(ProcessIntegerTypeProperty(property, logger));
                        break;
                    case "number":
                        json.Add(ProcessNumberTypeProperty(property, logger));
                        break;
                    case "string":
                        var token = ProcessStandardTypeProperty(property, logger);
                        if (token != null) json.Add(token);
                        break;
                    case "array":
                        json.Add(property.Key.ToString(), ProcessArrayProperty(property, logger));
                        break;
                    case "object":
                        json.Add(property.Key.ToString(), ProcessObjectProperty(property, logger));
                        break;
                    default:
                        break;
                }
            }
            return json;
        }

        public static JSchema StringToJsonSchema(string jsonSchema, ILogger logger)
        {
            try
            {
                JSchemaUrlResolver resolver = new JSchemaUrlResolver();
                return JSchema.Parse(jsonSchema, resolver);
            }
            catch (Exception ex)
            {
                logger.LogError(string.Format("Error on StringToJsonSchema. {0}", ex.Message));
                return null;
            }
        }

        private static JProperty ProcessStandardTypeProperty(KeyValuePair<string, JSchema> property, ILogger logger)
        {
            try
            {
                if (HasDefaultValue(property.Value))
                {
                    return new JProperty(property.Key, property.Value.Default.ToString());
                }
                else if (property.Value.Format.ToLower() == "date")
                {
                    return null;
                }
                else
                {
                    return new JProperty(property.Key, "");
                }
            }
            catch (Exception ex)
            {
                logger.LogError(string.Format("Error on ProcessStandardTypeProperty. {0}", ex.Message));
                return null;
            }
        }

        private static JProperty ProcessNumberTypeProperty(KeyValuePair<string, JSchema> property, ILogger logger)
        {
            try
            {
                if (HasDefaultValue(property.Value))
                {
                    return new JProperty(property.Key, (double)property.Value.Default);
                };
                return new JProperty(property.Key, null);
            }
            catch (Exception ex)
            {
                logger.LogError(string.Format("Error on ProcessStandardTypeProperty. {0}", ex.Message));
                return null;
            }
        }

        private static JProperty ProcessIntegerTypeProperty(KeyValuePair<string, JSchema> property, ILogger logger)
        {
            try
            {
                if (HasDefaultValue(property.Value))
                {
                    return new JProperty(property.Key, (int)property.Value.Default);
                };
                return new JProperty(property.Key, null);
            }
            catch (Exception ex)
            {
                logger.LogError(string.Format("Error on ProcessStandardTypeProperty. {0}", ex.Message));
                return null;
            }
        }

        private static JObject ProcessObjectProperty(KeyValuePair<string, JSchema> property, ILogger logger)
        {
            try
            {
                var jsonObject = new JObject();
                JSchema schema = StringToJsonSchema(property.Value.ToString(), logger);
                ProcessJsonSchemaProperties(jsonObject, schema, logger);
                return jsonObject;
            }
            catch (Exception ex)
            {
                logger.LogError(string.Format("Error on ProcessObjectProperty. {0}", ex.Message));
                return null;
            }
        }

        private static JArray ProcessArrayProperty(KeyValuePair<string, JSchema> property, ILogger logger)
        {
            try
            {
                var array = new JArray();
                JSchema schema = StringToJsonSchema(property.Value.ToString(), logger);
                if (schema.Default != null)
                {
                    array.Add(schema.Default.First);
                }
                return array;
            }
            catch (Exception ex)
            {
                logger.LogError(string.Format("Error on ProcessArrayProperty. {0}", ex.Message));
                return null;
            }
        }

        private static bool HasDefaultValue(JSchema propertyValue)
        {
            var defaultProperty = propertyValue.Default;
            if (defaultProperty != null) return true;
            return false;
        }
    }
}