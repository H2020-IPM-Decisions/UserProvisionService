using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using H2020.IPMDecisions.UPR.Core.Entities;

namespace H2020.IPMDecisions.UPR.Data.Core.Repositories
{
    public interface IUserWidgetRepository
    {
        Task<List<UserWidget>> FindByUserIdAsync(Guid userId);
        Task InitialCreation(Guid userId);
        void Update(List<UserWidget> entities);
    }
}