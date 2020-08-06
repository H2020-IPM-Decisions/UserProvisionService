using System.Collections.Generic;
using H2020.IPMDecisions.UPR.Core.Entities;

namespace H2020.IPMDecisions.UPR.Data.Core.Repositories
{
    public interface IUserFarmsRepository
    {
        void Delete(UserFarm entity);
        void DeleteRange(List<UserFarm> entities);
    }
}