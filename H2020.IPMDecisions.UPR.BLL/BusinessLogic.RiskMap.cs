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
                System.Console.WriteLine(language);
                var httpClient = new HttpClient();
                var response = await httpClient.GetAsync(string.Format("{0}?service=WMS&version=1.3.0&request=GetCapabilities&language={0}", riskMapListShortDto.WmsUrl, language));
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
                // Get layer names. For nibio is {0}.{1}.{2}

                // loop and get dates

                // Get legend from firstitem

                return GenericResponseBuilder.Success(riskMapListShortDto);
            }
            catch (Exception ex)
            {
                logger.LogError(string.Format("Error in BLL - GetRiskMapFilteredDataSource. {0}", ex.Message), ex);
                String innerMessage = (ex.InnerException != null) ? ex.InnerException.Message : "";
                return GenericResponseBuilder.NoSuccess<RiskMapFullDetailDto>(null, $"{ex.Message} InnerException: {innerMessage}");
            }

        }
    }
}