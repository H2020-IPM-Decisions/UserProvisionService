using System.Threading.Tasks;
using H2020.IPMDecisions.UPR.Core.Entities;
using H2020.IPMDecisions.UPR.Core.Enums;
using H2020.IPMDecisions.UPR.Core.ResourceParameters;

namespace H2020.IPMDecisions.UPR.Data.Core.Repositories
{
    public interface IUserProfileRepository : IRepositoryBase<UserProfile, UserProfileResourceParameter>
    {
        Task AddFarm(UserProfile userProfile, Farm farm, UserFarmTypeEnum userType = UserFarmTypeEnum.Unknown, bool isAuthorised = false);
    }
}