using System.Threading.Tasks;
using H2020.IPMDecisions.UPR.Core.Entities;

namespace H2020.IPMDecisions.UPR.Data.Core.Repositories
{
    public interface IWeatherHistoricalRepository : IRepositoryBase<WeatherHistorical>
    {
        Task<WeatherHistorical> FindByWeatherIdAsync(string id);
    }
}