using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using H2020.IPMDecisions.UPR.Core.Entities;
using H2020.IPMDecisions.UPR.Data.Core.Repositories;
using Microsoft.EntityFrameworkCore;

namespace H2020.IPMDecisions.UPR.Data.Persistence.Repositories
{
    internal class UserWidgetRepository : IUserWidgetRepository
    {
        private ApplicationDbContext context;
        public UserWidgetRepository(ApplicationDbContext context)
        {
            this.context = context;
        }

        public async Task<List<UserWidget>> FindByUserIdAsync(Guid userId)
        {
            return await this
                .context
                .UserWidget
                .Where(uw => uw.UserId == userId)
                .ToListAsync();
        }

        public async Task InitialCreation(Guid userId)
        {
            var widgets = await this.context.Widget.ToListAsync();
            var userWidgetsList = new List<UserWidget>();
            foreach (var widget in widgets)
            {
                var userWidget = new UserWidget()
                {
                    UserId = userId,
                    Widget = widget,
                    WidgetId = (int)widget.Id,
                    Allowed = true
                };
                userWidgetsList.Add(userWidget);
            }
            await this.context.AddRangeAsync(userWidgetsList);
        }

        public void Update(List<UserWidget> entities)
        {
            this.context.UpdateRange(entities);
        }
    }
}