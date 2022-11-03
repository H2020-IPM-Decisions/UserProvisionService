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
    internal class WeatherHistoricalRepository : IWeatherHistoricalRepository
    {
        private ApplicationDbContext context;

        public WeatherHistoricalRepository(ApplicationDbContext context)
        {
            this.context = context;
        }

        public void Create(WeatherHistorical entity)
        {
            this.context.Add(entity);
        }

        public void Delete(WeatherHistorical entity)
        {
            this.context.WeatherHistorical.Remove(entity);
        }

        public async Task<IEnumerable<WeatherHistorical>> FindAllAsync()
        {
            return await this.context
               .WeatherHistorical
               .ToListAsync<WeatherHistorical>();
        }

        public async Task<WeatherHistorical> FindByConditionAsync(Expression<Func<WeatherHistorical, bool>> expression)
        {
            return await this.context
                .WeatherHistorical
                .Where(expression)
                .FirstOrDefaultAsync();
        }

        public async Task<WeatherHistorical> FindByConditionAsync(Expression<Func<WeatherHistorical, bool>> expression, bool includeAssociatedData)
        {
            if (!includeAssociatedData)
            {
                return await FindByConditionAsync(expression);
            }

            return await this.context
                .WeatherHistorical
                .Where(expression)
                .FirstOrDefaultAsync();
        }

        public async Task<WeatherHistorical> FindByIdAsync(Guid id)
        {
            return await this.context
               .WeatherHistorical.
               SingleOrDefaultAsync(w =>
               w.Id == id);
        }

        public async Task<WeatherHistorical> FindByWeatherIdAsync(string id)
        {
            return await this.context
               .WeatherHistorical.
               SingleOrDefaultAsync(w =>
               w.WeatherId == id);
        }

        public void Update(WeatherHistorical entity)
        {
            this.context.Update(entity);
        }
    }
}