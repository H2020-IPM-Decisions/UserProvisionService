using System.Threading.Tasks;
using H2020.IPMDecisions.UPR.Core.Dtos;
using H2020.IPMDecisions.UPR.Core.Entities;

namespace H2020.IPMDecisions.UPR.BLL
{
    public partial class BusinessLogic : IBusinessLogic
    {
        #region Helpers
        private async Task<WeatherForecast> EnsureWeatherForecastExists(WeatherForecastForCreationDto weatherForecast)
        {
            var weatherStationAsEntity = await this
                                .dataService
                                .WeatherForecasts
                                .FindByWeatherIdAsync(weatherForecast.WeatherId);

            if (weatherStationAsEntity == null)
            {
                weatherStationAsEntity = this.mapper.Map<WeatherForecast>(weatherForecast);
                this.dataService.WeatherForecasts.Create(weatherStationAsEntity);
            }
            return weatherStationAsEntity;
        }

        private async Task<WeatherHistorical> EncodeNewWeatherHistoricalExists(WeatherHistoricalForCreationDto weatherHistoricalDto)
        {
            var weatherStationAsEntity = await this
                                .dataService
                                .WeatherHistoricals
                                .FindByWeatherIdAsync(weatherHistoricalDto.WeatherId);

            if (weatherStationAsEntity == null)
            {
                weatherStationAsEntity = this.mapper.Map<WeatherHistorical>(weatherHistoricalDto);
                this.dataService.WeatherHistoricals.Create(weatherStationAsEntity);
            }
            return weatherStationAsEntity;
        }
        #endregion
    }
}