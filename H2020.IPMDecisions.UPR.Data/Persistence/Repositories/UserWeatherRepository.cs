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
    internal class UserWeatherRepository : IUserWeatherRepository
    {
        private ApplicationDbContext context;
        public UserWeatherRepository(ApplicationDbContext context)
        {
            this.context = context;
        }

        public void Create(UserWeather entity)
        {
            this.context.Add(entity);
        }

        public void Delete(UserWeather entity)
        {
            this.context.Remove(entity);
        }

        public async Task<IEnumerable<UserWeather>> FindByUserIdAsync(Guid userId)
        {
            return await this
                .context
                .UserWeather
                .Where(uw => uw.UserId == userId)
                .ToListAsync();
        }

        public Task<IEnumerable<UserWeather>> FindAllAsync()
        {
            throw new NotImplementedException();
        }

        public async Task<UserWeather> FindByConditionAsync(Expression<Func<UserWeather, bool>> expression)
        {
            return await this.context
                .UserWeather
                .Where(expression)
                .FirstOrDefaultAsync();
        }

        public Task<UserWeather> FindByConditionAsync(Expression<Func<UserWeather, bool>> expression, bool includeAssociatedData)
        {
            throw new NotImplementedException();
        }

        public async Task<UserWeather> FindByIdAsync(Guid id)
        {
            return await this.context
                .UserWeather
                .Where(uw => uw.Id.Equals(id))
                .FirstOrDefaultAsync();
        }

        public void Update(UserWeather entity)
        {
            this.context.UserWeather.Update(entity);
        }
    }
}