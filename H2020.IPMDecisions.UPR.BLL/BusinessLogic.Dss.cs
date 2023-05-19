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
        public async Task<GenericResponse<IEnumerable<DssInformation>>> GetAllAvailableDssOnFarmLocation(DssListFilterDto dssListFilterDto, Guid userId)
        {
            try
            {
                var dssList = internalCommunicationProvider.GetAllListOfDssFilteredByCropsFromDssMicroservice(dssListFilterDto.CropCodes, dssListFilterDto.ExecutionType, dssListFilterDto.Country);
                var weatherParameters = internalCommunicationProvider.GetWeatherParametersAvailableByLocation(Math.Round(dssListFilterDto.LocationLatitude, 4), Math.Round(dssListFilterDto.LocationLongitude, 4));
                var eppoCodesData = await this.dataService.EppoCodes.GetEppoCodesAsync();
                var listOfModelIds = new List<(string, Guid)>();
                if (dssListFilterDto.DisplayIsSavedByUser)
                {
                    var listOfUserDss = await this
                        .dataService
                        .FieldCropPestDsses
                        .FindAllAsync(d =>
                            d.FieldCropPest.FieldCrop.Field.Farm.UserFarms.FirstOrDefault().UserId == userId);

                    if (dssListFilterDto.FarmIdSavedFilter != null && dssListFilterDto.FarmIdSavedFilter != Guid.Empty)
                    {
                        listOfUserDss = listOfUserDss
                            .Where(d => d.FieldCropPest.FieldCrop.Field.Farm.Id.Equals(dssListFilterDto.FarmIdSavedFilter))
                            .ToList();
                    }
                    listOfModelIds = listOfUserDss.Select(d => (d.CropPestDss.DssModelId, d.Id)).ToList();
                }

                var dssListResult = await dssList;
                if (dssListResult == null || dssListResult.Count() == 0) return GenericResponseBuilder.Success<IEnumerable<DssInformation>>(dssListResult);
                var weatherParametersResult = await weatherParameters;

                if (weatherParametersResult == null)
                {
                    return GenericResponseBuilder.NoSuccess<IEnumerable<DssInformation>>(null, this.jsonStringLocalizer["weather.internal_error"].ToString());
                }
                var weatherParametersAsList = weatherParametersResult.Select(wx => wx);
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

                    // For observation models?
                    if (model.Input != null && model.Input.WeatherParameters == null)
                        model.WeatherParametersValidated = true;

                    if (model.Input != null && model.Input.WeatherParameters != null)
                        AreAllWeatherParametersAvailable(weatherParametersAsList, model);
                    model.AlreadySavedByUser = false;
                    if (dssListFilterDto.DisplayIsSavedByUser)
                    {
                        var modelExist = listOfModelIds.FirstOrDefault(d => d.Item1 == model.Id);
                        if (modelExist.Item1 != null)
                        {
                            model.AlreadySavedByUser = true;
                            model.DssDatabaseId = modelExist.Item2;
                        }
                    }

                    DistinctEppoCodes(eppoCodesData, model);
                }

                foreach (var modelToDelete in listModelsToDelete)
                {
                    var dssInformation = dssListResult.Where(d => d.DssModelInformation.Any(m => m.Id == modelToDelete.Id)).FirstOrDefault();
                    dssInformation.DssModelInformation.Remove(modelToDelete);
                }
                foreach (var dss in dssListResult)
                {
                    if (!string.IsNullOrEmpty(dss.LogoUrl) && (!dss.LogoUrl.StartsWith("http")))
                    {
                        var dssApiUrl = config["MicroserviceInternalCommunication:DssApiUrl"];
                        dss.LogoUrl = string.Format("{0}{1}",
                            dssApiUrl,
                            dss.LogoUrl);
                    }

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

        private static void DistinctEppoCodes(List<EppoCode> eppoCodesResult, DssModelInformation model)
        {
            if (eppoCodesResult is null) return;
            if (model.Crops != null)
            {
                var cropEppoCodesToRemove = new List<string>();
                var cropEppoCodesLanguage = new List<EppoCodeDto>();
                foreach (var eppoCode in model.Crops)
                {
                    var eppoAsDto = new EppoCodeDto()
                    {
                        EppoCode = eppoCode,
                        Languages = EppoCodesHelper.GetNameFromEppoCodeData(eppoCodesResult, "crop", eppoCode)
                    };

                    var languageValue = eppoAsDto.Languages.FirstOrDefault().Value.ToString();
                    if (!cropEppoCodesLanguage.Any(c => c.Languages.Any(l => l.Value.Contains(languageValue))))
                        cropEppoCodesLanguage.Add(eppoAsDto);
                    else
                        cropEppoCodesToRemove.Add(eppoCode);
                }
                foreach (var toRemove in cropEppoCodesToRemove)
                {
                    model.Crops.Remove(toRemove);
                }
            }
            if (model.Pests != null)
            {
                var pestEppoCodesToRemove = new List<string>();
                var pestEppoCodesLanguage = new List<EppoCodeDto>();
                foreach (var eppoCode in model.Pests)
                {
                    var eppoAsDto = new EppoCodeDto()
                    {
                        EppoCode = eppoCode,
                        Languages = EppoCodesHelper.GetNameFromEppoCodeData(eppoCodesResult, "pest", eppoCode)
                    };
                    var languageValue = eppoAsDto.Languages.FirstOrDefault().Value.ToString();
                    if (!pestEppoCodesLanguage.Any(c => c.Languages.Any(l => l.Value.Contains(languageValue))))
                        pestEppoCodesLanguage.Add(eppoAsDto);
                    else
                        pestEppoCodesToRemove.Add(eppoCode);
                }
                foreach (var toRemove in pestEppoCodesToRemove)
                {
                    model.Pests.Remove(toRemove);
                }
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