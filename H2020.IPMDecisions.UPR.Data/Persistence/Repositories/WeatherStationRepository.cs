using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using H2020.IPMDecisions.UPR.Core.Entities;
using H2020.IPMDecisions.UPR.Data.Core.Repositories;
using Microsoft.EntityFrameworkCore;

namespace H2020.IPMDecisions.UPR.Data.Persistence.Repositories
{
    internal class WeatherStationRepository : IWeatherStationRepository
    {
        private ApplicationDbContext context;

        public WeatherStationRepository(ApplicationDbContext context)
        {
            this.context = context;
        }

        public void Create(WeatherStation entity)
        {
            this.context.Add(entity);
        }

        public void Delete(WeatherStation entity)
        {
            this.context.WeatherStation.Remove(entity);
        }

        public async Task<IEnumerable<WeatherStation>> FindAllAsync()
        {
            return await this.context
               .WeatherStation
               .ToListAsync<WeatherStation>();
        }

        public async Task<WeatherStation> FindByConditionAsync(Expression<Func<WeatherStation, bool>> expression)
        {
            return await this.context
                .WeatherStation
                .Where(expression)
                .FirstOrDefaultAsync();
        }

        public async Task<WeatherStation> FindByConditionAsync(Expression<Func<WeatherStation, bool>> expression, bool includeAssociatedData)
        {
            if (!includeAssociatedData)
            {
                return await FindByConditionAsync(expression);
            }

            return await this.context
                .WeatherStation
                .Where(expression)
                .Include(w => w.FarmWeatherStations)
                .FirstOrDefaultAsync();
        }

        public async Task<WeatherStation> FindByIdAsync(Guid id)
        {
            return await this.context
               .WeatherStation.
               SingleOrDefaultAsync(w =>
               w.Id == id.ToString());
        }

        public async Task<WeatherStation> FindByIdAsync(string id)
        {
            return await this.context
               .WeatherStation.
               SingleOrDefaultAsync(w =>
               w.Id == id);
        }

        public void Update(WeatherStation entity)
        {
            this.context.Update(entity);
        }
    }
}