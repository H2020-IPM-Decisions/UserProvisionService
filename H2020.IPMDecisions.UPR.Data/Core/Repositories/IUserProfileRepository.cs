using H2020.IPMDecisions.UPR.Core.Entities;
using H2020.IPMDecisions.UPR.Core.ResourceParameters;

namespace H2020.IPMDecisions.UPR.Data.Core.Repositories
{
    public interface IUserProfileRepository : IRepositoryBase<UserProfile, UserProfileResourceParameter>
    {
        void AddFarm(UserProfile userProfile, Farm farm);
    }
}