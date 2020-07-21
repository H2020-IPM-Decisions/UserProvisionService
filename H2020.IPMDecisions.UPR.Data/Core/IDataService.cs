using System;
using System.Threading.Tasks;
using H2020.IPMDecisions.UPR.Data.Core.Repositories;

namespace H2020.IPMDecisions.UPR.Data.Core
{
    public interface IDataService : IDisposable
    {
        Task CompleteAsync();
        IUserProfileRepository UserProfiles { get; }
        IFarmRepository Farms { get; }
        IFieldRepository Fields { get; }
    }
}