using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using H2020.IPMDecisions.UPR.Core.Dtos;
using H2020.IPMDecisions.UPR.Core.Entities;
using H2020.IPMDecisions.UPR.Core.Models;
using Microsoft.AspNetCore.Http;

namespace H2020.IPMDecisions.UPR.BLL
{
    public interface IBusinessLogic
    {
        #region User Farms
        Task<GenericResponse<FarmDto>> LinkNewFarmToUserProfile(FarmForCreationDto farmForCreation, Guid userId, string mediaType);
        #endregion

        #region Farms
        Task<GenericResponse<FarmDto>> GetFarmById(Guid id, string fields, HttpContext context, string mediaType);
        #endregion

        #region Field

        #endregion

        #region  UserProfile
        Task<GenericResponse<IDictionary<string, object>>> AddNewUserProfile(Guid userId, UserProfileForCreationDto userProfileForCreation, string mediaType);
        Task<GenericResponse<UserProfileDto>> AddNewUserProfile(Guid userId, UserProfileForCreationDto userProfileForCreation);
        Task<GenericResponse> DeleteUserProfileClient(Guid userId);
        Task<GenericResponse<UserProfile>> GetUserProfile(Guid userId);
        Task<GenericResponse<IDictionary<string, object>>> GetUserProfileDto(Guid userId, string fields, string mediaType);
        UserProfileForCreationDto MapToUserProfileForCreation(UserProfileForUpdateDto userProfileDto);
        UserProfileForUpdateDto MapToUserProfileForUpdateDto(UserProfile userProfile);
        Task<GenericResponse> UpdateUserProfile(UserProfile userProfile, UserProfileForUpdateDto userProfileToPatch);
        #endregion
    }
}