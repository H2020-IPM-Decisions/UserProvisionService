using System;
using System.Threading.Tasks;
using H2020.IPMDecisions.UPR.Core.Dtos;
using H2020.IPMDecisions.UPR.Core.Models;

namespace H2020.IPMDecisions.UPR.BLL
{
    public interface IBusinessLogic
    {
        #region  UserProfile
        Task<GenericResponse> AddNewUserProfile(Guid userId, UserProfileForCreationDto userProfileForCreation);
        #endregion
    }
}