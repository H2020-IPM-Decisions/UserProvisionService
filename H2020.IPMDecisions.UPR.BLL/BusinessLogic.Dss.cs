using System;
using System.Collections.Generic;
using System.Linq;
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
        public async Task<GenericResponse<IEnumerable<DssInformation>>> GetAllAvailableDssOnFarmLocation(DssListFilterDto dssListFilterDto)
        {
            try
            {
                if (string.IsNullOrEmpty(dssListFilterDto.CropCodes))
                {
                    return GenericResponseBuilder.NoSuccess<IEnumerable<DssInformation>>(null, "Please select at least one crop");
                }

                var dssList = internalCommunicationProvider.GetAllListOfDssFilteredByCropsFromDssMicroservice(dssListFilterDto.CropCodes, dssListFilterDto.ExecutionType, dssListFilterDto.Country);
                var weatherParameters = internalCommunicationProvider.GetWeatherParametersAvailableByLocation(dssListFilterDto.LocationLongitude, dssListFilterDto.LocationLatitude);
                var eppoCodesData = this.dataService.EppoCodes.GetEppoCodesAsync();

                var dssListResult = await dssList;
                if (dssListResult.Count() == 0) return GenericResponseBuilder.Success<IEnumerable<DssInformation>>(dssListResult);
                var weatherParametersResult = await weatherParameters;

                var weatherParametersAsList = weatherParametersResult.Select(wx => wx);
                var eppoCodesResult = await eppoCodesData;
                var listModelsToDelete = new List<DssModelInformation>();
                foreach (DssModelInformation model in dssListResult.SelectMany(d => d.DssModelInformation))
                {
                    if (!string.IsNullOrEmpty(dssListFilterDto.Country) && model.ValidSpatial != null
                            && model.ValidSpatial.Countries != null
                            && !model.ValidSpatial.Countries.Any(c => c.ToUpper() == dssListFilterDto.Country.ToUpper()))
                    {
                        listModelsToDelete.Add(model);
                        continue;
                    }

                    if (model.Input != null && model.Input.WeatherParameters != null)
                        AreAllWeatherParametersAvailable(weatherParametersAsList, model);

                    CreateEppoCodesDto(eppoCodesResult, model);
                }

                foreach (var modelToDelete in listModelsToDelete)
                {
                    var dssInformation = dssListResult.Where(d => d.DssModelInformation.Any(m => m.Id == modelToDelete.Id)).FirstOrDefault();
                    dssInformation.DssModelInformation.Remove(modelToDelete);
                }

                return GenericResponseBuilder.Success<IEnumerable<DssInformation>>(dssListResult);
            }
            catch (Exception ex)
            {
                logger.LogError(string.Format("Error in BLL - GetAllAvailableDssOnFarmLocation. {0}", ex.Message), ex);
                String innerMessage = (ex.InnerException != null) ? ex.InnerException.Message : "";
                return GenericResponseBuilder.NoSuccess<IEnumerable<DssInformation>>(null, $"{ex.Message} InnerException: {innerMessage}");
            }
        }

        private static void CreateEppoCodesDto(List<EppoCode> eppoCodesResult, DssModelInformation model)
        {
            if (model.Crops != null)
            {
                foreach (var eppoCode in model.Crops)
                {
                    var cropAsDto = new EppoCodeDto()
                    {
                        EppoCode = eppoCode,
                        Languages = EppoCodesHelper.GetNameFromEppoCodeData(eppoCodesResult, "crop", eppoCode)
                    };
                    model.CropsDto.Add(cropAsDto);
                }
                // Do CropsDto distinct
            }
            if (model.Pests != null)
            {
                foreach (var eppoCode in model.Pests)
                {
                    var cropAsDto = new EppoCodeDto()
                    {
                        EppoCode = eppoCode,
                        Languages = EppoCodesHelper.GetNameFromEppoCodeData(eppoCodesResult, "pest", eppoCode)
                    };
                    model.PestsDto.Add(cropAsDto);
                }
                // Do PestsDto distinct
            }
        }

        private static void AreAllWeatherParametersAvailable(IEnumerable<int> weatherParametersAsList, DssModelInformation model)
        {
            var modelWeatherParameters = model
                .Input
                .WeatherParameters
                .Select(w => w.ParameterCode)
                .ToList();
            model.WeatherParametersValidated = modelWeatherParameters.All(w => weatherParametersAsList.Any(m => w == m));
        }
    }
}