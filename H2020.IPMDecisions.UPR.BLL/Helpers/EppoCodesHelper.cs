using System.Collections.Generic;
using System.Linq;
using H2020.IPMDecisions.UPR.Core.Entities;
using H2020.IPMDecisions.UPR.Core.Models;
using Newtonsoft.Json;

namespace H2020.IPMDecisions.UPR.BLL.Helpers
{
    public static class EppoCodesHelper
    {
        public static CropAndPestNameAsDictionaries GetCropPestEppoCodesNames(
            List<EppoCode> eppoCodesData,
            string cropEppoCode,
            string pestEppoCode)
        {
            return new CropAndPestNameAsDictionaries()
            {
                // crop and pest text match default loaded data
                CropLanguages = GetNameFromEppoCodeData(eppoCodesData, "crop", cropEppoCode),
                PestLanguages = GetNameFromEppoCodeData(eppoCodesData, "pest", pestEppoCode)
            };
        }

        public static IDictionary<string, string> GetNameFromEppoCodeData(
            List<EppoCode> eppoCodesData,
            string eppoCodeType,
            string eppoCodeFilter,
            string languageFilter = "en")
        {
            var cropData = eppoCodesData.Where(e => e.Type == eppoCodeType).FirstOrDefault();
            if (cropData == null) return null;
            var eppoCodesOnType = JsonConvert.DeserializeObject<List<IDictionary<string, string>>>(cropData.Data);
            var selectedEppoCodeDto = FilterEppoCodesByEppoCode(eppoCodeFilter, eppoCodesOnType);
            if (selectedEppoCodeDto == null) return null;
            return DoLanguageFilter(languageFilter, selectedEppoCodeDto);
        }

        public static IDictionary<string, string> FilterEppoCodesByEppoCode(string eppoCodeFilter, List<IDictionary<string, string>> eppoCodesOnType)
        {
            return eppoCodesOnType
                .Where(d =>
                    d.TryGetValue("EPPOCode", out string value)
                    && value is string i && i.ToLower() == eppoCodeFilter.ToLower())
                .FirstOrDefault();
        }

        public static IDictionary<string, string> DoLanguageFilter(string languageFilter, IDictionary<string, string> eppoCodeLanguages)
        {
            if (!string.IsNullOrEmpty(languageFilter))
                return FilterEppoCodeLanguage(languageFilter, eppoCodeLanguages);
            return eppoCodeLanguages;
        }

        private static IDictionary<string, string> FilterEppoCodeLanguage(string languageFilter, IDictionary<string, string> eppoCode)
        {
            // default language: latin (la)            
            return eppoCode
                .Where(e => e.Key == "la"
                || e.Key.ToLower() == languageFilter.ToLower())
                .ToDictionary(e => e.Key, e => e.Value);
        }

        internal static IDictionary<string, string> NoLanguagesAvailable(string languageFilter, string eppoCode)
        {
            var languages = new Dictionary<string, string>();
            languages.Add("la", eppoCode);
            return languages;
        }
    }
}