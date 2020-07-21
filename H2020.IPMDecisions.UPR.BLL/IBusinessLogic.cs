using H2020.IPMDecisions.UPR.Core.Dtos;
using H2020.IPMDecisions.UPR.Core.Entities;
using H2020.IPMDecisions.UPR.Core.Models;
using H2020.IPMDecisions.UPR.Core.ResourceParameters;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace H2020.IPMDecisions.UPR.BLL
{
    public interface IBusinessLogic
    {
        #region User Farms
        Task<GenericResponse<FarmDto>> LinkNewFarmToUserProfile(FarmForCreationDto farmForCreationDto, Guid id, Guid userId);
        Task<GenericResponse<FarmDto>> LinkNewFarmToUserProfile(FarmForCreationDto farmForCreationDto, Guid userId, string mediaType);
        #endregion

        #region Farms
        Task<GenericResponse> DeleteFarm(Guid id, HttpContext httpContext);
        Task<GenericResponse<Farm>> GetFarm(Guid id, HttpContext httpContext);
        Task<GenericResponse<FarmDto>> GetFarmDto(Guid id, HttpContext httpContext, string fields, string mediaType);
        Task<GenericResponse<ShapedDataWithLinks>> GetFarms(Guid userId, FarmResourceParameter resourceParameter, string mediaType);
        FarmForCreationDto MapToFarmForCreation(FarmForUpdateDto farmDto);
        FarmForUpdateDto MapToFarmForUpdateDto(Farm farm);
        Task<GenericResponse> UpdateFarm(Farm farm, FarmForUpdateDto farmToPatch);

        #endregion

        #region Field
        Task<GenericResponse<FieldDto>> AddNewField(FieldForCreationDto fieldForCreationDto, HttpContext httpContext, string mediaType);
        Task<GenericResponse<ShapedDataWithLinks>> GetFields(Guid farmId, FieldResourceParameter resourceParameter, string mediaType);
        Task<GenericResponse<FieldDto>> GetFieldDto(Guid id, string fields, string mediaType);
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