using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using H2020.IPMDecisions.UPR.Core.Dtos;
using H2020.IPMDecisions.UPR.Core.Models;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace H2020.IPMDecisions.UPR.BLL
{
    public partial class BusinessLogic : IBusinessLogic
    {
        public async Task<GenericResponse<List<EppoCodeTypeDto>>> GetAllEppoCodes()
        {
            try
            {
                var eppoCodes = await this.dataService.EppoCodes.GetEppoCodesAsync();

                List<EppoCodeTypeDto> dataToReturn = new List<EppoCodeTypeDto>();
                foreach (var item in eppoCodes)
                {
                    EppoCodeTypeDto eppoCodeType = new EppoCodeTypeDto();
                    eppoCodeType.EppoCodeType = item.Type;
                    var eppoCodesOnType = JsonConvert.DeserializeObject<List<IDictionary<string, string>>>(item.Data);
                    foreach (var eppoCode in eppoCodesOnType)
                    {
                        EppoCodeDto eppoCodeDto = new EppoCodeDto();
                        eppoCodeDto.EppoCode = eppoCode["EPPOCode"];
                        eppoCodeDto.Languages = eppoCode;
                        eppoCodeType.EppoCodesDto.Add(eppoCodeDto);
                    }
                    dataToReturn.Add(eppoCodeType);
                }              
                
                return GenericResponseBuilder.Success<List<EppoCodeTypeDto>>(dataToReturn);
            }
            catch (Exception ex)
            {
                logger.LogError(string.Format("Error in BLL - DeleteFarm. {0}", ex.Message));
                String innerMessage = (ex.InnerException != null) ? ex.InnerException.Message : "";
                return GenericResponseBuilder.NoSuccess<List<EppoCodeTypeDto>>(null, $"{ex.Message} InnerException: {innerMessage}");
            }
        }
        #region Helpers
        #endregion
    }
}