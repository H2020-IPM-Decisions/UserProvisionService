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
        public async Task<GenericResponse<FieldDssResultDetailedDto>> GetFieldCropPestDssById(Guid id, Guid userId)
        {
            try
            {
                var dss = await this.dataService.FieldCropPestDsses.FindByIdAsync(id);
                if (dss == null) return GenericResponseBuilder.NotFound<FieldDssResultDetailedDto>();

                var dssUserId = dss.FieldCropPest.FieldCrop.Field.Farm.UserFarms.FirstOrDefault().UserId;
                if (userId != dssUserId) return GenericResponseBuilder.NotFound<FieldDssResultDetailedDto>();

                FieldDssResultDetailedDto dataToReturn = await CreateDetailedResultToReturn(dss);
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

                if (dssResults != null && dssResults.Count != 0)
                {
                    var listOfDss = await this.internalCommunicationProvider.GetAllListOfDssFromDssMicroservice();
                    if (listOfDss != null && listOfDss.Count() != 0)
                    {
                        foreach (var dss in dssResults)
                        {
                            // ToDo check if DssVersion needed with Tor-Einar
                            var selectedDss = listOfDss.Where(d => d.Id == dss.DssId).FirstOrDefault();
                        }
                    }
                }
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

        private async Task<FieldDssResultDetailedDto> CreateDetailedResultToReturn(FieldCropPestDss dss)
        {
            var dataToReturn = this.mapper.Map<FieldDssResultDetailedDto>(dss);
            var dssInformation = await internalCommunicationProvider
                            .GetDssModelInformationFromDssMicroservice(dss.CropPestDss.DssId, dss.CropPestDss.DssModelId);

            if (dssInformation == null) return dataToReturn;
            dataToReturn.DssTypeOfDecision = dssInformation.TypeOfDecision;
            dataToReturn.DssTypeOfOutput = dssInformation.TypeOfOutput;
            dataToReturn.DssDescription = dssInformation.Description;
            dataToReturn.DssDescriptionUrl = dssInformation.DescriptionUrl;
            dataToReturn.WarningMessage = dssInformation.Output.WarningStatusInterpretation;

            if (!dataToReturn.IsValid) return dataToReturn;

            var dssFullOutputAsObject = JsonConvert.DeserializeObject<DssModelOutputInformation>(dataToReturn.DssFullResult);
            dataToReturn.OutputTimeStart = dssFullOutputAsObject.TimeStart;
            dataToReturn.OutputTimeEnd = dssFullOutputAsObject.TimeEnd;
            dataToReturn.Interval = dssFullOutputAsObject.Interval;
            dataToReturn.ResultParametersWidth = dssFullOutputAsObject.ResultParameters.Count;

            var locationResultData = dssFullOutputAsObject.LocationResult.FirstOrDefault();
            IEnumerable<List<double>> dataLastSevenDays = new List<List<double>>();

            if (locationResultData != null)
            {
                //Take last 7 days of Data
                var maxDaysOutput = 7;
                dataLastSevenDays = locationResultData.Data.TakeLast(maxDaysOutput);
                dataToReturn.ResultParametersLength = dataLastSevenDays.Count();
                dataToReturn.WarningStatusPerDay = locationResultData.WarningStatus.TakeLast(maxDaysOutput).ToList();
            };

            List<string> labels = CreateResultParametersLabels(dataToReturn.OutputTimeEnd, dataLastSevenDays.Count());

            for (int i = 0; i < dssFullOutputAsObject.ResultParameters.Count; i++)
            {
                var parameterCode = dssFullOutputAsObject.ResultParameters[i];
                var resultParameter = new ResultParameters();
                resultParameter.Labels = labels;

                resultParameter.Code = parameterCode;
                var parameterInformationFromDss = dssInformation
                        .Output
                        .ResultParameters
                        .Where(n => n.Id == parameterCode)
                        .FirstOrDefault();
                if (parameterInformationFromDss != null)
                {
                    resultParameter.Title = parameterInformationFromDss.Title;
                    resultParameter.Description = parameterInformationFromDss.Description;

                    if (parameterInformationFromDss.ChartInfo != null)
                    {
                        resultParameter.ChartInformation = this.mapper.Map<DssParameterChartInformation>(parameterInformationFromDss.ChartInfo);
                    };
                }

                foreach (var dataForParameters in dataLastSevenDays)
                {
                    var data = dataForParameters[i];
                    resultParameter.Data.Add(data);
                }
                dataToReturn.ResultParameters.Add(resultParameter);
            }
            return dataToReturn;
        }

        private List<string> CreateResultParametersLabels(string outputTimeEnd, int days)
        {
            if (outputTimeEnd is null) return null;

            var isADate = DateTime.TryParse(outputTimeEnd, out DateTime dateTime);
            if (!isADate) return null;

            var labelsList = new List<string>();
            for (int i = days - 1; i >= 0; i--)
            {
                labelsList.Add(dateTime.AddDays(-i).ToShortDateString());
            }
            return labelsList;
        }
    }
}