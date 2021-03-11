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
    internal class WeatherDataSourcesRepository : IWeatherDataSourcesRepository
    {
        private ApplicationDbContext context;

        public WeatherDataSourcesRepository(ApplicationDbContext context)
        {
            this.context = context;
        }

        public void Create(WeatherDataSource entity)
        {
            this.context.Add(entity);
        }

        public void Delete(WeatherDataSource entity)
        {
            this.context.WeatherDataSource.Remove(entity);
        }

        public async Task<IEnumerable<WeatherDataSource>> FindAllAsync()
        {
            return await this.context
                .WeatherDataSource
                .ToListAsync<WeatherDataSource>();
        }

        public async Task<WeatherDataSource> FindByConditionAsync(Expression<Func<WeatherDataSource, bool>> expression)
        {
            return await this.context
                .WeatherDataSource
                .Where(expression)
                .FirstOrDefaultAsync();
        }

        public async Task<WeatherDataSource> FindByConditionAsync(Expression<Func<WeatherDataSource, bool>> expression, bool includeAssociatedData)
        {
            if (!includeAssociatedData)
            {
                return await FindByConditionAsync(expression);
            }

            return await this.context
                .WeatherDataSource
                .Where(expression)
                .FirstOrDefaultAsync();
        }

        public async Task<WeatherDataSource> FindByIdAsync(Guid id)
        {
            return await this.context
               .WeatherDataSource.
               SingleOrDefaultAsync(w =>
               w.Id.ToString() == id.ToString());
        }

        public void Update(WeatherDataSource entity)
        {
            this.context.Update(entity);
        }
    }
}