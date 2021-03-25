using System.Threading.Tasks;
using H2020.IPMDecisions.UPR.Core.Dtos;
using H2020.IPMDecisions.UPR.Core.Entities;

namespace H2020.IPMDecisions.UPR.BLL
{
    public partial class BusinessLogic : IBusinessLogic
    {
        #region Helpers
        private WeatherDataSource EncodeNewWeatherDataSourcePassword(WeatherForManipulationDto weatherDataSourceDto)
        {
            if (weatherDataSourceDto.Credentials == null) return this.mapper.Map<WeatherDataSource>(weatherDataSourceDto);
            weatherDataSourceDto.Credentials.Password = _encryption.Encrypt(weatherDataSourceDto.Credentials.Password);
            return this.mapper.Map<WeatherDataSource>(weatherDataSourceDto);
        }

        private WeatherStation EncodeNewWeatherStationPassword(WeatherForManipulationDto weatherStationDto)
        {
            if (weatherStationDto.Credentials == null) return this.mapper.Map<WeatherStation>(weatherStationDto);
            weatherStationDto.Credentials.Password = _encryption.Encrypt(weatherStationDto.Credentials.Password);
            return this.mapper.Map<WeatherStation>(weatherStationDto);
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