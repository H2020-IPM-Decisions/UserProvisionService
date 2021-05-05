using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using H2020.IPMDecisions.UPR.Core.Dtos;
using H2020.IPMDecisions.UPR.Core.Models;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace H2020.IPMDecisions.UPR.BLL
{
    public partial class BusinessLogic : IBusinessLogic
    {
        public async Task<GenericResponse<FieldDssResultDetailedDto>> GetFieldCropPestDssById(Guid id, Guid userId)
        {
            try
            {
                var dss = await this.dataService.FieldCropPestDsses.FindByIdAsync(id);
                if (dss == null) return GenericResponseBuilder.NotFound<FieldDssResultDetailedDto>();

                var dssUserId = dss.FieldCropPest.FieldCrop.Field.Farm.UserFarms.FirstOrDefault().UserId;
                if (userId != dssUserId) return GenericResponseBuilder.NotFound<FieldDssResultDetailedDto>();

                var dataToReturn = this.mapper.Map<FieldDssResultDetailedDto>(dss);

                // ToDo Tidy up
                var dssInformation = await internalCommunicationProvider
                    .GetDssInformationFromDssMicroservice(dss.CropPestDss.DssId, dss.CropPestDss.DssModelId);
                dataToReturn.WarningMessage = dssInformation.Output.WarningStatusInterpretation;

                var dssOutput = JsonConvert.DeserializeObject<DssModelOutputInformation>(dataToReturn.DssFullResult);

                dataToReturn.OutputTimeStart = dssOutput.TimeStart;
                dataToReturn.OutputTimeEnd = dssOutput.TimeEnd;
                dataToReturn.Interval = dssOutput.Interval;
                //Take last 7 days of Data
                var dataLastSevenDays = dssOutput.LocationResult.FirstOrDefault().Data.TakeLast(7);

                for (int i = 0; i < dssOutput.ResultParameters.Count; i++)
                {
                    var resultParameter = new ResultParameters();
                    var parameterCode = dssOutput.ResultParameters[i];
                    var parameterInformationFromDss = dssInformation
                        .Output
                        .ResultParameters
                        .Where(n => n.Id == parameterCode)
                        .FirstOrDefault();
                    if (parameterInformationFromDss != null)
                    {
                        resultParameter.Code = parameterInformationFromDss.Id;
                        resultParameter.Title = parameterInformationFromDss.Title;
                        resultParameter.Description = parameterInformationFromDss.Description;
                    }
                    else
                    {
                        resultParameter.Code = parameterCode;
                    }

                    foreach (var dataForParameters in dataLastSevenDays)
                    {
                        var data = dataForParameters[i];
                        resultParameter.Data.Add(data);
                    }
                    dataToReturn.ResultParameters.Add(resultParameter);
                }
                return GenericResponseBuilder.Success<FieldDssResultDetailedDto>(dataToReturn);
            }
            catch (Exception ex)
            {
                logger.LogError(string.Format("Error in BLL - GetFieldCropPestDssById. {0}", ex.Message), ex);
                String innerMessage = (ex.InnerException != null) ? ex.InnerException.Message : "";
                return GenericResponseBuilder.NoSuccess<FieldDssResultDetailedDto>(null, $"{ex.Message} InnerException: {innerMessage}");
            }
        }

        public async Task<GenericResponse> UpdateFieldCropPestDssById(Guid id, Guid userId, FieldCropPestDssForUpdateDto fieldCropPestDssForUpdateDto)
        {
            try
            {
                var dss = await this.dataService.FieldCropPestDsses.FindByIdAsync(id);
                if (dss == null) return GenericResponseBuilder.NotFound<FieldCropPestDssDto>();

                var dssUserId = dss.FieldCropPest.FieldCrop.Field.Farm.UserFarms.FirstOrDefault().UserId;
                if (userId != dssUserId) return GenericResponseBuilder.NotFound<FieldCropPestDssDto>();

                this.mapper.Map(fieldCropPestDssForUpdateDto, dss);
                this.dataService.FieldCropPestDsses.Update(dss);
                await this.dataService.CompleteAsync();
                return GenericResponseBuilder.Success();
            }
            catch (Exception ex)
            {
                logger.LogError(string.Format("Error in BLL - UpdateFieldCropPestDssById. {0}", ex.Message), ex);
                String innerMessage = (ex.InnerException != null) ? ex.InnerException.Message : "";
                return GenericResponseBuilder.NoSuccess($"{ex.Message} InnerException: {innerMessage}");
            }
        }

        public async Task<GenericResponse<IEnumerable<FieldDssResultDto>>> GetAllDssResults(Guid userId)
        {
            try
            {
                var dssResults = await this.dataService.DssResult.GetAllDssResults(userId);
                var dssResultsToReturn = this.mapper.Map<IEnumerable<FieldDssResultDto>>(dssResults);
                return GenericResponseBuilder.Success<IEnumerable<FieldDssResultDto>>(dssResultsToReturn);
            }
            catch (Exception ex)
            {
                logger.LogError(string.Format("Error in BLL - GetAllUserFieldCropPestDss. {0}", ex.Message), ex);
                String innerMessage = (ex.InnerException != null) ? ex.InnerException.Message : "";
                return GenericResponseBuilder.NoSuccess<IEnumerable<FieldDssResultDto>>(null, $"{ex.Message} InnerException: {innerMessage}");
            }
        }

        public async Task<GenericResponse> DeleteDss(Guid id, Guid userId)
        {
            try
            {
                var dss = await this.dataService.FieldCropPestDsses.FindByIdAsync(id);
                if (dss == null) return GenericResponseBuilder.Success();

                var dssUserId = dss.FieldCropPest.FieldCrop.Field.Farm.UserFarms.FirstOrDefault().UserId;
                if (userId != dssUserId) return GenericResponseBuilder.Success();

                this.dataService.FieldCropPestDsses.Delete(dss);
                await this.dataService.CompleteAsync();

                return GenericResponseBuilder.Success();
            }
            catch (Exception ex)
            {
                logger.LogError(string.Format("Error in BLL - DeleteDss. {0}", ex.Message), ex);
                String innerMessage = (ex.InnerException != null) ? ex.InnerException.Message : "";
                return GenericResponseBuilder.NoSuccess($"{ex.Message} InnerException: {innerMessage}");
            }
        }
    }
}