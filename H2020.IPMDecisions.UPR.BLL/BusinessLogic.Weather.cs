using System;
using System.Threading.Tasks;
using H2020.IPMDecisions.UPR.Core.Entities;

namespace H2020.IPMDecisions.UPR.BLL
{
    public partial class BusinessLogic : IBusinessLogic
    {
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