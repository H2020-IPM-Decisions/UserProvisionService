using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using H2020.IPMDecisions.UPR.Core.Entities;
using H2020.IPMDecisions.UPR.Data.Core;
using Microsoft.EntityFrameworkCore;

namespace H2020.IPMDecisions.UPR.Data.Persistence.Repositories
{
    internal class WeatherForecastRepository : IWeatherForecastRepository
    {
        private ApplicationDbContext context;

        public WeatherForecastRepository(ApplicationDbContext context)
        {
            this.context = context;
        }

        public void Create(WeatherForecast entity)
        {
            this.context.Add(entity); ;
        }

        public void Delete(WeatherForecast entity)
        {
            this.context.WeatherForecast.Remove(entity);
        }

        public async Task<IEnumerable<WeatherForecast>> FindAllAsync()
        {
            return await this.context
               .WeatherForecast
               .ToListAsync<WeatherForecast>();
        }

        public async Task<WeatherForecast> FindByConditionAsync(Expression<Func<WeatherForecast, bool>> expression)
        {
            return await this.context
                .WeatherForecast
                .Where(expression)
                .FirstOrDefaultAsync();
        }

        public async Task<WeatherForecast> FindByConditionAsync(Expression<Func<WeatherForecast, bool>> expression, bool includeAssociatedData)
        {
            if (!includeAssociatedData)
            {
                return await FindByConditionAsync(expression);
            }

            return await this.context
                .WeatherForecast
                .Where(expression)
                .FirstOrDefaultAsync();
        }

        public async Task<WeatherForecast> FindByWeatherIdAsync(string id)
        {
            return await this.context
               .WeatherForecast.
               SingleOrDefaultAsync(w =>
               w.WeatherId == id);
        }

        public async Task<WeatherForecast> FindByIdAsync(Guid id)
        {
            return await this.context
               .WeatherForecast.
               SingleOrDefaultAsync(w =>
               w.Id == id);
        }

        public void Update(WeatherForecast entity)
        {
            this.context.Update(entity);
        }
    }
}