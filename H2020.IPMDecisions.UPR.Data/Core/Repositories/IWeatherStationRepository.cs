using System.Threading.Tasks;
using H2020.IPMDecisions.UPR.Core.Entities;

namespace H2020.IPMDecisions.UPR.Data.Core.Repositories
{
    public interface IWeatherStationRepository : IRepositoryBase<WeatherStation>
    {
        Task<WeatherStation> FindByIdAsync(string id);
    }
}