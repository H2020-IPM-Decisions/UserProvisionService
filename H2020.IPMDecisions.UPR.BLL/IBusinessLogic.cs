using System;
using System.Threading.Tasks;
using H2020.IPMDecisions.UPR.Core.Dtos;
using H2020.IPMDecisions.UPR.Core.Entities;
using H2020.IPMDecisions.UPR.Core.Models;

namespace H2020.IPMDecisions.UPR.BLL
{
    public interface IBusinessLogic
    {
        #region  UserProfile
        Task<GenericResponse<UserProfileDto>> AddNewUserProfile(Guid userId, UserProfileForCreationDto userProfileForCreation);
        Task<GenericResponse> DeleteUserProfileClient(Guid userId);
        Task<GenericResponse<UserProfile>> GetUserProfile(Guid userId);
        Task<GenericResponse<UserProfileDto>> GetUserProfileDto(Guid userId);
        UserProfileForCreationDto MapToUserProfileForCreation(UserProfileForUpdateDto userProfileDto);
        UserProfileForUpdateDto MapToUserProfileForUpdateDto(UserProfile userProfile);
        Task<GenericResponse> UpdateUserProfile(UserProfile userProfile, UserProfileForUpdateDto userProfileToPatch);
        #endregion
    }
}