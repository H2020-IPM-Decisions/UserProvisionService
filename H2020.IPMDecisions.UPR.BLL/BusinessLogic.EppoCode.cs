using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using H2020.IPMDecisions.UPR.BLL.Helpers;
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
                    EppoCodeTypeDto eppoCodeType = await EppoCodeToEppoCodeTypeDto(type, languageFilter: Thread.CurrentThread.CurrentCulture.Name);
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
                EppoCodeTypeDto dataToReturn = await EppoCodeToEppoCodeTypeDto(type, eppoCode, Thread.CurrentThread.CurrentCulture.Name);
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

                EppoCodeTypeDto dataToReturn = await EppoCodeToEppoCodeTypeDto(eppoCodeAsEntity, languageFilter: Thread.CurrentThread.CurrentCulture.Name);
                return GenericResponseBuilder.Success<EppoCodeTypeDto>(dataToReturn);
            }
            catch (Exception ex)
            {
                logger.LogError(string.Format("Error in BLL - CreateEppoCode. {0}", ex.Message));
                String innerMessage = (ex.InnerException != null) ? ex.InnerException.Message : "";
                return GenericResponseBuilder.NoSuccess<EppoCodeTypeDto>(null, $"{ex.Message} InnerException: {innerMessage}");
            }
        }

        public async Task<GenericResponse> UpdateEppoCodeType(string eppoCodeType, EppoCodeForUpdateDto eppoCodeForUpdateDto)
        {
            try
            {
                var eppoCodeTypeFiltered = await this.dataService.EppoCodes.GetEppoCodesAsync(eppoCodeType);
                if (eppoCodeTypeFiltered.Count == 0) return GenericResponseBuilder.NotFound<EppoCodeTypeDto>();

                var eppoCodeToUpdate = eppoCodeTypeFiltered.FirstOrDefault();
                eppoCodeToUpdate.Data = eppoCodeForUpdateDto.EppoCodes;
                this.dataService.EppoCodes.Update(eppoCodeToUpdate);
                await this.dataService.CompleteAsync();
                return GenericResponseBuilder.Success();
            }
            catch (Exception ex)
            {
                logger.LogError(string.Format("Error in BLL - UpdateEppoCodeType. {0}", ex.Message));
                String innerMessage = (ex.InnerException != null) ? ex.InnerException.Message : "";
                return GenericResponseBuilder.NoSuccess($"{ex.Message} InnerException: {innerMessage}");
            }
        }

        #region Helpers
        private async Task<EppoCodeTypeDto> EppoCodeToEppoCodeTypeDto(EppoCode type, string eppoCodeFilter = "", string languageFilter = "en")
        {
            EppoCodeTypeDto eppoCodeType = this.mapper.Map<EppoCodeTypeDto>(type);
            var eppoCodesOnType = JsonConvert.DeserializeObject<List<IDictionary<string, string>>>(type.Data);
            var eppoCodesFromDssService = await this.internalCommunicationProvider.GetListOfEppoCodesFromDssMicroservice(type.Type.ToLower());

            if (!string.IsNullOrEmpty(eppoCodeFilter))
            {
                EppoCodeDto eppoCodeDto = new EppoCodeDto();
                var selectedEppoCodeDto = EppoCodesHelper.FilterEppoCodesByEppoCode(eppoCodeFilter, eppoCodesOnType);
                if (selectedEppoCodeDto == null) return null;

                eppoCodeDto.Languages = EppoCodesHelper.DoLanguageFilter(languageFilter, selectedEppoCodeDto);
                eppoCodeDto.EppoCode = selectedEppoCodeDto["EPPOCode"];
                eppoCodeType.EppoCodesDto.Add(eppoCodeDto);
                return eppoCodeType;
            }

            foreach (var eppoCode in eppoCodesFromDssService)
            {
                EppoCodeDto eppoCodeDto = new EppoCodeDto();
                eppoCodeDto.EppoCode = eppoCode;

                var eppoCodeFromFullList = eppoCodesOnType.FirstOrDefault(l => l.Values.Contains(eppoCode));

                if (eppoCodeFromFullList != null)
                    eppoCodeDto.Languages = EppoCodesHelper.DoLanguageFilter(languageFilter, eppoCodeFromFullList);
                else
                    eppoCodeDto.Languages = EppoCodesHelper.NoLanguagesAvailable(languageFilter, eppoCode);

                eppoCodeType.EppoCodesDto.Add(eppoCodeDto);
            }
            return eppoCodeType;
        }
        #endregion
    }
}