using System;
using System.Threading.Tasks;
using H2020.IPMDecisions.UPR.Core.Dtos;
using H2020.IPMDecisions.UPR.Core.Entities;

namespace H2020.IPMDecisions.UPR.BLL
{
    public partial class BusinessLogic : IBusinessLogic
    {
        #region Helpers
        private async Task<WeatherForecast> EnsureWeatherForecastExists(string weatherForecastId)
        {
            var weatherStationAsEntity = await this
                                .dataService
                                .WeatherForecasts
                                .FindByWeatherIdAsync(weatherForecastId);

            if (weatherStationAsEntity == null)
            {
                var weatherInformation = await this.internalCommunicationProvider
                    .GetWeatherProviderInformationFromWeatherMicroservice(weatherForecastId);
                if (weatherInformation == null) throw new NullReferenceException(string.Format("Weather service with ID '{0}' do not exist on weather microservice.", weatherForecastId));

                weatherStationAsEntity = this.mapper.Map<WeatherForecast>(weatherInformation);
                this.dataService.WeatherForecasts.Create(weatherStationAsEntity);
            }
            return weatherStationAsEntity;
        }

        private async Task<WeatherHistorical> EnsureWeatherHistoricalExists(string weatherHistoricalId)
        {
            var weatherStationAsEntity = await this
                                .dataService
                                .WeatherHistoricals
                                .FindByWeatherIdAsync(weatherHistoricalId);

            if (weatherStationAsEntity == null)
            {
                var weatherInformation = await this.internalCommunicationProvider
                   .GetWeatherProviderInformationFromWeatherMicroservice(weatherHistoricalId);
                if (weatherInformation == null) throw new NullReferenceException(string.Format("Weather service with ID '{0}' do not exist on weather microservice.", weatherHistoricalId));

                weatherStationAsEntity = this.mapper.Map<WeatherHistorical>(weatherInformation);
                this.dataService.WeatherHistoricals.Create(weatherStationAsEntity);
            }
            return weatherStationAsEntity;
        }
        #endregion
    }
}