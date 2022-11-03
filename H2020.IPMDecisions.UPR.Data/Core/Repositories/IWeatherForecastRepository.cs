using System.Threading.Tasks;
using H2020.IPMDecisions.UPR.Core.Entities;
using H2020.IPMDecisions.UPR.Data.Core.Repositories;

namespace H2020.IPMDecisions.UPR.Data.Core
{
    public interface IWeatherForecastRepository : IRepositoryBase<WeatherForecast>
    {
        Task<WeatherForecast> FindByWeatherIdAsync(string id);
    }
}