using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Schema;
using System;
using System.Collections.Generic;

namespace H2020.IPMDecisions.UPR.BLL.Helpers
{
    public static class JsonSchemaToJson
    {
        public static string ToJsonString(string jsonSchema, ILogger logger, bool isDemoVersion = false)
        {
            try
            {
                var jsonObject = ToJsonObject(jsonSchema, logger, isDemoVersion);
                return jsonObject.ToString();
            }
            catch (Exception ex)
            {
                logger.LogError(string.Format("Error converting ToJsonString. {0}", ex.Message));
                return ex.Message.ToString();
            }
        }

        public static JObject ToJsonObject(string jsonSchema, ILogger logger, bool isDemoVersion = false)
        {
            try
            {
                var jsonObject = new JObject();
                JSchema schema = StringToJsonSchema(jsonSchema, logger, isDemoVersion);
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

        public static JSchema StringToJsonSchema(string jsonSchema, ILogger logger, bool isDemoVersion = false)
        {
            try
            {
                JSchemaUrlResolver resolver = new JSchemaUrlResolver();
                jsonSchema = jsonSchema.Replace("ipmdecisions.nibio.no", "platform.ipmdecisions.net");
                if (isDemoVersion) jsonSchema = jsonSchema.Replace("platform.ipmdecisions.net", "demo.ipmdecisions.net");
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
                else if (property.Value.Format != null && property.Value.Format.ToLower() == "date")
                {
                    return null;
                }
                else if (property.Value.Enum != null && property.Value.Enum.Count != 0)
                {
                    return new JProperty(property.Key, property.Value.Enum[0].ToString());
                }
                else
                {
                    return new JProperty(property.Key, "");
                }
            }
            catch (Exception ex)
            {
                logger.LogError(string.Format("Error on ProcessStandardTypeProperty. Property:{0}. Error: {1}", property, ex.Message));
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
                logger.LogError(string.Format("Error on ProcessNumberTypeProperty. {0}", ex.Message));
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
                }
                else if (property.Value.Enum != null && property.Value.Enum.Count != 0)
                {
                    return new JProperty(property.Key, property.Value.Enum[0].ToString());
                }
                return new JProperty(property.Key, null);
            }
            catch (Exception ex)
            {
                logger.LogError(string.Format("Error on ProcessIntegerTypeProperty. {0}", ex.Message));
                return null;
            }
        }

        private static JObject ProcessObjectProperty(KeyValuePair<string, JSchema> property, ILogger logger, bool isDemoVersion = false)
        {
            try
            {
                var jsonObject = new JObject();
                JSchema schema = StringToJsonSchema(property.Value.ToString(), logger, isDemoVersion);
                ProcessJsonSchemaProperties(jsonObject, schema, logger);
                return jsonObject;
            }
            catch (Exception ex)
            {
                logger.LogError(string.Format("Error on ProcessObjectProperty. {0}", ex.Message));
                return null;
            }
        }

        private static JArray ProcessArrayProperty(KeyValuePair<string, JSchema> property, ILogger logger, bool isDemoVersion = false)
        {
            try
            {
                var array = new JArray();
                JSchema schema = StringToJsonSchema(property.Value.ToString(), logger, isDemoVersion);
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