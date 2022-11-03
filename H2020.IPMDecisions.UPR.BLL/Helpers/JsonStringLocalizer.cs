using System.IO;
using System.Threading;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Localization;
using Newtonsoft.Json;

namespace H2020.IPMDecisions.UPR.BLL.Helpers
{
    public class JsonStringLocalizer : IJsonStringLocalizer
    {
        private readonly JsonSerializer jsonSerializer = new JsonSerializer();
        private readonly IDistributedCache distributedCache;

        public JsonStringLocalizer(IDistributedCache distributedCache)
        {
            this.distributedCache = distributedCache;
        }

        public LocalizedString this[string name]
        {
            get
            {
                string value = GetLocalizedString(name);
                return new LocalizedString(name, value ?? name, value == null);
            }
        }

        public LocalizedString this[string name, params object[] arguments]
        {
            get
            {
                var value = this[name];

                return !value.ResourceNotFound
                    ? new LocalizedString(name, string.Format(value.Value, arguments), false)
                    : value;
            }
        }

        private string GetLocalizedString(string key)
        {
            string relativeFilePath = $"Resources/location.{Thread.CurrentThread.CurrentCulture.Name}.json";
            string fullFilePath = Path.GetFullPath(relativeFilePath);

            if (!File.Exists(fullFilePath))
            {
                relativeFilePath = $"Resources/location.en.json";
                fullFilePath = Path.GetFullPath(relativeFilePath);
            }

            if (File.Exists(fullFilePath))
            {
                string cacheKey = $"locale_{Thread.CurrentThread.CurrentCulture.Name}_{key}";
                string cacheValue = distributedCache.GetString(cacheKey);
                if (!string.IsNullOrEmpty(cacheValue)) return cacheValue;

                string result = GetJsonValue(key, fullFilePath);
                if (!string.IsNullOrEmpty(result)) distributedCache.SetString(cacheKey, result);
                return result;
            }
            return default;
        }

        private string GetJsonValue(string propertyName, string filePath, bool isDefaultFile = false)
        {
            if (propertyName == null) return default;
            if (filePath == null) return default;
            using (var str = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read))
            using (var sReader = new StreamReader(str))
            using (var reader = new JsonTextReader(sReader))
            {
                while (reader.Read())
                {
                    if (reader.TokenType == JsonToken.PropertyName && reader.Path as string == propertyName)
                    {
                        reader.Read();
                        return jsonSerializer.Deserialize<string>(reader);
                    }
                }
                if (isDefaultFile) return default;

                // try again with default language
                filePath = $"Resources/location.en.json";
                string fullFilePath = Path.GetFullPath(filePath);
                if (!File.Exists(fullFilePath)) return default;

                return GetJsonValue(propertyName, fullFilePath, true);
            }
        }
    }
}