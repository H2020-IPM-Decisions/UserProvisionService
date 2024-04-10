using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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