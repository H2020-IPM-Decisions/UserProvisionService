using System.Threading.Tasks;
using H2020.IPMDecisions.UPR.Core.Dtos;
using H2020.IPMDecisions.UPR.Core.Entities;

namespace H2020.IPMDecisions.UPR.BLL
{
    public partial class BusinessLogic : IBusinessLogic
    {
        #region Helpers
        private async Task EnsureWeatherDataSourcesExists(WeatherDataSourceDto weatherDataSourceDto)
        {
            var weatherDataSourceExist = await this
                                .dataService
                                .WeatherDataSources
                                .FindByIdAsync(weatherDataSourceDto.Id);

            if (weatherDataSourceExist == null)
            {
                var weatherDataSourceAsEntity = this.mapper.Map<WeatherDataSource>(weatherDataSourceDto);
                this.dataService.WeatherDataSources.Create(weatherDataSourceAsEntity);
            }
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