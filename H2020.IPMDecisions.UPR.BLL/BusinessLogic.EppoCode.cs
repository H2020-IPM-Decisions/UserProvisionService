using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using H2020.IPMDecisions.UPR.Core.Dtos;
using H2020.IPMDecisions.UPR.Core.Entities;
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
                var eppoCodesTypes = await this.dataService.EppoCodes.GetEppoCodesAsync();

                List<EppoCodeTypeDto> dataToReturn = new List<EppoCodeTypeDto>();
                foreach (var type in eppoCodesTypes)
                {
                    EppoCodeTypeDto eppoCodeType = EppoCodeToEppoCodeTypeDto(type);
                    dataToReturn.Add(eppoCodeType);
                }
                return GenericResponseBuilder.Success<List<EppoCodeTypeDto>>(dataToReturn);
            }
            catch (Exception ex)
            {
                logger.LogError(string.Format("Error in BLL - GetAllEppoCodes. {0}", ex.Message));
                String innerMessage = (ex.InnerException != null) ? ex.InnerException.Message : "";
                return GenericResponseBuilder.NoSuccess<List<EppoCodeTypeDto>>(null, $"{ex.Message} InnerException: {innerMessage}");
            }
        }

        public async Task<GenericResponse<List<string>>> GetEppoCodeTypes()
        {
            try
            {
                List<string> eppoCodeTypes = await this.dataService.EppoCodes.GetEppoCodeTypesAsync();
                return GenericResponseBuilder.Success<List<string>>(eppoCodeTypes);
            }
            catch (Exception ex)
            {
                logger.LogError(string.Format("Error in BLL - GetEppoCodeTypes. {0}", ex.Message));
                String innerMessage = (ex.InnerException != null) ? ex.InnerException.Message : "";
                return GenericResponseBuilder.NoSuccess<List<string>>(null, $"{ex.Message} InnerException: {innerMessage}");
            }
        }

        public async Task<GenericResponse<EppoCodeTypeDto>> GetEppoCode(string eppoCodeType, string eppoCode)
        {
            try
            {
                var eppoCodeTypeFiltered = await this.dataService.EppoCodes.GetEppoCodesAsync(eppoCodeType);
                if (eppoCodeTypeFiltered.Count == 0) return GenericResponseBuilder.NotFound<EppoCodeTypeDto>();

                var type = eppoCodeTypeFiltered.FirstOrDefault();
                EppoCodeTypeDto dataToReturn = EppoCodeToEppoCodeTypeDto(type, eppoCode);
                if (dataToReturn == null) return GenericResponseBuilder.NotFound<EppoCodeTypeDto>();
                return GenericResponseBuilder.Success<EppoCodeTypeDto>(dataToReturn);
            }
            catch (Exception ex)
            {
                logger.LogError(string.Format("Error in BLL - GetEppoCode. {0}", ex.Message));
                String innerMessage = (ex.InnerException != null) ? ex.InnerException.Message : "";
                return GenericResponseBuilder.NoSuccess<EppoCodeTypeDto>(null, $"{ex.Message} InnerException: {innerMessage}");
            }
        }

        public async Task<GenericResponse<EppoCodeTypeDto>> CreateEppoCodeType(EppoCodeForCreationDto eppoCodeForCreationDto)
        {
            try
            {
                var eppoCodeAsEntity = this.mapper.Map<EppoCode>(eppoCodeForCreationDto);
                this.dataService.EppoCodes.Create(eppoCodeAsEntity);
                await this.dataService.CompleteAsync();

                EppoCodeTypeDto dataToReturn = EppoCodeToEppoCodeTypeDto(eppoCodeAsEntity);
                return GenericResponseBuilder.Success<EppoCodeTypeDto>(dataToReturn);
            }
            catch (System.Exception ex)
            {
                logger.LogError(string.Format("Error in BLL - GetEppoCode. {0}", ex.Message));
                String innerMessage = (ex.InnerException != null) ? ex.InnerException.Message : "";
                return GenericResponseBuilder.NoSuccess<EppoCodeTypeDto>(null, $"{ex.Message} InnerException: {innerMessage}");
            }
        }

        #region Helpers
        private EppoCodeTypeDto EppoCodeToEppoCodeTypeDto(EppoCode type, string eppoCodeFilter = "")
        {
            EppoCodeTypeDto eppoCodeType = this.mapper.Map<EppoCodeTypeDto>(type);
            var eppoCodesOnType = JsonConvert.DeserializeObject<List<IDictionary<string, string>>>(type.Data);

            if (!string.IsNullOrEmpty(eppoCodeFilter))
            {
                EppoCodeDto eppoCodeDto = new EppoCodeDto();
                var selectedEppoCodeDto = eppoCodesOnType
                      .Where(d =>
                          d.TryGetValue("EPPOCode", out string value)
                          && value is string i && i.ToLower() == eppoCodeFilter.ToLower())
                      .FirstOrDefault();

                if (selectedEppoCodeDto == null) return null;

                eppoCodeDto.Languages = selectedEppoCodeDto;
                eppoCodeDto.EppoCode = selectedEppoCodeDto["EPPOCode"];
                eppoCodeType.EppoCodesDto.Add(eppoCodeDto);
                return eppoCodeType;
            }

            foreach (var eppoCode in eppoCodesOnType)
            {
                EppoCodeDto eppoCodeDto = new EppoCodeDto();
                eppoCodeDto.EppoCode = eppoCode["EPPOCode"];
                eppoCodeDto.Languages = eppoCode;
                eppoCodeType.EppoCodesDto.Add(eppoCodeDto);
            }
            return eppoCodeType;
        }
        #endregion
    }
}