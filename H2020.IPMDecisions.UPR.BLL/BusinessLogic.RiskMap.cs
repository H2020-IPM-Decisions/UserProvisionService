using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Serialization;
using H2020.IPMDecisions.UPR.Core.Dtos;
using H2020.IPMDecisions.UPR.Core.Models;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace H2020.IPMDecisions.UPR.BLL
{
    public partial class BusinessLogic : IBusinessLogic
    {
        public async Task<GenericResponse<List<RiskMapBaseDto>>> GetRiskMapDataSources()
        {
            try
            {
                RiskMapProvider riskMapList = await internalCommunicationProvider.GetAllTheRiskMapsFromDssMicroservice();
                List<RiskMapFullDetailDto> riskMapListAsFullDto = riskMapList.ToRiskMapBaseDto();

                var riskMapListShortDto = this.mapper.Map<List<RiskMapBaseDto>>(riskMapListAsFullDto);
                return GenericResponseBuilder.Success(riskMapListShortDto);
            }
            catch (Exception ex)
            {
                logger.LogError(string.Format("Error in BLL - GetRiskMapDataSources. {0}", ex.Message), ex);
                String innerMessage = (ex.InnerException != null) ? ex.InnerException.Message : "";
                return GenericResponseBuilder.NoSuccess<List<RiskMapBaseDto>>(null, $"{ex.Message} InnerException: {innerMessage}");
            }

        }
        public async Task<GenericResponse<RiskMapFullDetailDto>> GetRiskMapFilteredDataSource(string providerId, string id)
        {
            try
            {
                RiskMapProvider riskMapList = await internalCommunicationProvider.GetAllTheRiskMapsFromDssMicroservice();
                List<RiskMapFullDetailDto> riskMapListAsFullDto = riskMapList.ToRiskMapBaseDto();

                var riskMapListShortDto = riskMapListAsFullDto.Where(rm => rm.ProviderId.Equals(providerId) && rm.Id.Equals(id)).FirstOrDefault();

                if (riskMapListShortDto is null)
                    return GenericResponseBuilder.NotFound<RiskMapFullDetailDto>();

                var language = Thread.CurrentThread.CurrentCulture.Name;
                if (language.ToLower().Equals("no")) language = "nb"; // Exception added because Nibio's server work with NB instead of NO
                var httpClient = new HttpClient();
                var response = await httpClient.GetAsync(string.Format("{0}?service=WMS&version=1.3.0&request=GetCapabilities&language={1}", riskMapListShortDto.WmsUrl, language));
                if (!response.IsSuccessStatusCode)
                    return GenericResponseBuilder.NoSuccess<RiskMapFullDetailDto>(null, "Error getting the risk maps");

                // Read and deserialize XML
                var xmlAsString = await response.Content.ReadAsStringAsync();
                XmlSerializer serializer = new XmlSerializer(typeof(WmsCapabilities));
                WmsCapabilities wmsCapabilities = new WmsCapabilities();
                using (StringReader stringReader = new StringReader(xmlAsString))
                {
                    wmsCapabilities = (WmsCapabilities)serializer.Deserialize(stringReader);
                }

                if (providerId.Equals("nibio", StringComparison.InvariantCultureIgnoreCase))
                {
                    riskMapListShortDto.MapConfiguration = GetLayerFromNibioWMS(wmsCapabilities);
                }

                return GenericResponseBuilder.Success(riskMapListShortDto);
            }
            catch (Exception ex)
            {
                logger.LogError(string.Format("Error in BLL - GetRiskMapFilteredDataSource. {0}", ex.Message), ex);
                String innerMessage = (ex.InnerException != null) ? ex.InnerException.Message : "";
                return GenericResponseBuilder.NoSuccess<RiskMapFullDetailDto>(null, $"{ex.Message} InnerException: {innerMessage}");
            }

        }

        private MapConfiguration GetLayerFromNibioWMS(WmsCapabilities wmsCapabilities)
        {
            var mapConfiguration = new MapConfiguration
            {
                Name = wmsCapabilities.Service.Name,
                Title = wmsCapabilities.Service.Title,
                Abstract = wmsCapabilities.Service.Abstract,
                Projection = wmsCapabilities.Capability.Layer.CRS.FirstOrDefault()
            };

            HashSet<string> uniqueLayerNames = new HashSet<string>();
            HashSet<string> uniqueLayerDates = new HashSet<string>();
            var fullListOfLayers = wmsCapabilities.Capability.Layer.Layers;
            foreach (var layer in fullListOfLayers)
            {
                string[] parts = layer.Name.Split('.');
                string layerNameWithoutDate = string.Join(".", parts.Take(2));
                string layerDates = parts.LastOrDefault();
                uniqueLayerNames.Add(layerNameWithoutDate);
                uniqueLayerDates.Add(layerDates);
            }

            foreach (var uniqueName in uniqueLayerNames)
            {
                var layerConfiguration = new LayerConfiguration
                {
                    Name = uniqueName,
                    Dates = uniqueLayerDates.ToList()
                };

                var firstLayer = fullListOfLayers.Where(l => l.Name.StartsWith(uniqueName)).FirstOrDefault();
                string[] parts = firstLayer.Name.Split('.');
                string layerDate = parts.LastOrDefault();
                layerConfiguration.Title = firstLayer.Title.Replace(layerDate, "");
                layerConfiguration.LegendURL = firstLayer.Styles.FirstOrDefault().LegendURLs.FirstOrDefault().OnlineResource.Href;
                layerConfiguration.LegendMetadata = JsonConvert.DeserializeObject<dynamic>(firstLayer.Abstract);

                mapConfiguration.LayersConfiguration.Add(layerConfiguration);
            }
            return mapConfiguration;
        }
    }
}