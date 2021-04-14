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

        private async Task EnsureWeatherStationExists(WeatherStationDto weatherStationDto)
        {
            var weatherStationExist = await this
                                .dataService
                                .WeatherStations
                                .FindByIdAsync(weatherStationDto.Id);

            if (weatherStationExist == null)
            {
                var weatherStationAsEntity = this.mapper.Map<WeatherStation>(weatherStationDto);
                this.dataService.WeatherStations.Create(weatherStationAsEntity);
            }
        }
        #endregion
    }
}