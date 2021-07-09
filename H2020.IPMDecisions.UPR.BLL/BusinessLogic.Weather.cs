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
                // ToDo
                // call weather service with ID and create weather object - Waiting for Service on WX API
                var weatherForecastDefault = new WeatherForecastForCreationDto()
                {
                    WeatherId = "FMI weather forecasts",
                    Name = "FMI weather forecasts",
                    Url = "https://ipmdecisions.nibio.no/api/wx/rest/weatheradapter/fmi/forecasts"
                };
                weatherStationAsEntity = this.mapper.Map<WeatherForecast>(weatherForecastDefault);
                this.dataService.WeatherForecasts.Create(weatherStationAsEntity);
            }
            return weatherStationAsEntity;
        }

        private async Task<WeatherHistorical> EncodeWeatherHistoricalExists(string weatherHistoricalId)
        {
            var weatherStationAsEntity = await this
                                .dataService
                                .WeatherHistoricals
                                .FindByWeatherIdAsync(weatherHistoricalId);

            if (weatherStationAsEntity == null)
            {
                //ToDo
                // call weather service with ID and create weather object - Waiting for Service on WX API
                var weatherHistoricalDefault = new WeatherHistoricalForCreationDto()
                {
                    WeatherId = "Finnish Meteorological Institute measured data",
                    Name = "Finnish Meteorological Institute measured data",
                    Url = "https://ipmdecisions.nibio.no/api/wx/rest/weatheradapter/fmi/"
                };
                weatherStationAsEntity = this.mapper.Map<WeatherHistorical>(weatherHistoricalDefault);
                this.dataService.WeatherHistoricals.Create(weatherStationAsEntity);
            }
            return weatherStationAsEntity;
        }
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

        private async Task<WeatherHistorical> EncodeWeatherHistoricalExists(WeatherHistoricalForCreationDto weatherHistoricalDto)
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