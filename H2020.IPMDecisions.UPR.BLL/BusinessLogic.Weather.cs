using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using H2020.IPMDecisions.UPR.Core.Dtos;
using H2020.IPMDecisions.UPR.Core.Entities;
using H2020.IPMDecisions.UPR.Core.Enums;
using H2020.IPMDecisions.UPR.Core.Models;
using Microsoft.Extensions.Logging;

namespace H2020.IPMDecisions.UPR.BLL
{
    public partial class BusinessLogic : IBusinessLogic
    {
        public async Task<GenericResponse<List<WeatherBaseDto>>> GetWeatherDataSources()
        {
            try
            {
                var listWeatherServices = await this.internalCommunicationProvider.GetListWeatherProviderInformationFromWeatherMicroservice();
                if (listWeatherServices == null) return GenericResponseBuilder.NoSuccess<List<WeatherBaseDto>>(null);
                var filteredList = listWeatherServices.Where(wx => wx.AuthenticationType == WeatherAuthenticationTypeEnum.Credentials).ToList();
                var dssApiUrl = config["MicroserviceInternalCommunication:DssApiUrl"];
                var dataToReturn = this.mapper.Map<List<WeatherBaseDto>>(filteredList, opt =>
                    {
                        opt.Items["host"] = string.Format("{0}", dssApiUrl);
                    });
                return GenericResponseBuilder.Success<List<WeatherBaseDto>>(dataToReturn);
            }
            catch (Exception ex)
            {
                logger.LogError(string.Format("Error in BLL - GetWeatherDataSources. {0}", ex.Message), ex);
                String innerMessage = (ex.InnerException != null) ? ex.InnerException.Message : "";
                return GenericResponseBuilder.NoSuccess<List<WeatherBaseDto>>(null, $"{ex.Message} InnerException: {innerMessage}");
            }
        }

        #region Helpers
        private async Task<WeatherForecast> EnsureWeatherForecastExists(string weatherForecastId, string hostName)
        {
            var weatherStationAsEntity = await this
                                .dataService
                                .WeatherForecasts
                                .FindByWeatherIdAsync(weatherForecastId);

            if (weatherStationAsEntity == null)
            {
                var weatherInformation = await this.internalCommunicationProvider
                    .GetWeatherProviderInformationFromWeatherMicroservice(weatherForecastId);
                if (weatherInformation == null) throw new NullReferenceException(this.jsonStringLocalizer["weather.missing_service", weatherForecastId].ToString());

                weatherStationAsEntity = this.mapper.Map<WeatherForecast>(weatherInformation, opt =>
                    {
                        opt.Items["host"] = hostName;
                    });
                this.dataService.WeatherForecasts.Create(weatherStationAsEntity);
            }
            return weatherStationAsEntity;
        }

        private async Task<WeatherHistorical> EnsureWeatherHistoricalExists(string weatherHistoricalId, string hostName)
        {
            var weatherStationAsEntity = await this
                                .dataService
                                .WeatherHistoricals
                                .FindByWeatherIdAsync(weatherHistoricalId);

            if (weatherStationAsEntity == null)
            {
                var weatherInformation = await this.internalCommunicationProvider
                   .GetWeatherProviderInformationFromWeatherMicroservice(weatherHistoricalId);
                if (weatherInformation == null) throw new NullReferenceException(this.jsonStringLocalizer["weather.missing_service", weatherHistoricalId].ToString());

                weatherStationAsEntity = this.mapper.Map<WeatherHistorical>(weatherInformation, opt =>
                    {
                        opt.Items["host"] = hostName;
                    });
                this.dataService.WeatherHistoricals.Create(weatherStationAsEntity);
            }
            return weatherStationAsEntity;
        }
        #endregion
    }
}