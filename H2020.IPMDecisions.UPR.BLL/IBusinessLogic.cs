using H2020.IPMDecisions.UPR.Core.Dtos;
using H2020.IPMDecisions.UPR.Core.Entities;
using H2020.IPMDecisions.UPR.Core.Models;
using H2020.IPMDecisions.UPR.Core.ResourceParameters;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace H2020.IPMDecisions.UPR.BLL
{
    public interface IBusinessLogic
    {
        #region User Farms
        Task<GenericResponse<IDictionary<string, object>>> LinkNewFarmToUserProfile(FarmForCreationDto farmForCreationDto, Guid id, Guid userId);
        Task<GenericResponse<IDictionary<string, object>>> LinkNewFarmToUserProfile(FarmForCreationDto farmForCreationDto, Guid userId, string mediaType);
        #endregion

        #region Crop Decision
        Task<GenericResponse<IDictionary<string, object>>> AddNewFieldCropDecision(CropPestDssForCreationDto cropPestDssForCreationDto, HttpContext httpContext, string mediaType);
        Task<GenericResponse> DeleteFieldCropDecision(Guid id, HttpContext httpContext);
        GenericResponse<IDictionary<string, object>> GetFieldCropDecision(Guid id, HttpContext httpContext, string mediaType);
        Task<GenericResponse<ShapedDataWithLinks>> GetFieldCropDecisions(FieldCropPestDssResourceParameter resourceParameter, HttpContext httpContext, string mediaType);
        #endregion

        #region Crop Pest
        Task<GenericResponse<IDictionary<string, object>>> AddNewFieldCropPest(CropPestForCreationDto cropPestForCreationDto, HttpContext httpContext, string mediaType);
        Task<GenericResponse> DeleteFieldCropPest(Guid id, Guid fieldId, HttpContext httpContext);
        Task<GenericResponse<IDictionary<string, object>>> GetFieldCropPest(Guid id, Guid fieldId, string mediaType, HttpContext httpContext);
        Task<GenericResponse<ShapedDataWithLinks>> GetFieldCropPests(Guid fieldId, FieldCropPestResourceParameter resourceParameter, string mediaType);
        #endregion

        #region Farms
        Task<GenericResponse> DeleteFarm(HttpContext httpContext);
        Task<GenericResponse<Farm>> GetFarm(Guid id, HttpContext httpContext);
        GenericResponse<IDictionary<string, object>> GetFarmDto(Guid id, HttpContext httpContext, FarmResourceParameter resourceParameter, string mediaType);
        Task<GenericResponse<ShapedDataWithLinks>> GetFarms(Guid userId, FarmResourceParameter resourceParameter, string mediaType);
        FarmForCreationDto MapToFarmForCreation(FarmForUpdateDto farmDto);
        FarmForUpdateDto MapToFarmForUpdateDto(Farm farm);
        Task<GenericResponse> UpdateFarm(Farm farm, FarmForUpdateDto farmToPatch, JsonPatchDocument<FarmForUpdateDto> patchDocument);
        #endregion

        #region FarmDss
        Task<GenericResponse<FarmDssDto>> AddNewFarmDss(FarmDssForCreationDto farmDssDto, HttpContext httpContext, string mediaType);
        #endregion

        #region Field
        Task<GenericResponse<IDictionary<string, object>>> AddNewField(FieldForCreationDto fieldForCreationDto, HttpContext httpContext, string mediaType);
        Task<GenericResponse<FieldDto>> AddNewField(FieldForCreationDto fieldForCreationDto, HttpContext httpContext, Guid id);
        Task<GenericResponse> DeleteField(Guid id);
        Task<GenericResponse<ShapedDataWithLinks>> GetFields(Guid farmId, FieldResourceParameter resourceParameter, string mediaType);
        Task<GenericResponse<IDictionary<string, object>>> GetFieldDto(Guid id, FieldResourceParameter resourceParameter, string mediaType);
        Task<GenericResponse<Field>> GetField(Guid id, HttpContext httpContext);
        FieldForCreationDto MapToFieldForCreation(FieldForUpdateDto fieldForUpdateDto);
        FieldForUpdateDto MapToFieldForUpdateDto(Field field);
        Task<GenericResponse> UpdateField(Field field, FieldForUpdateDto fieldToPatch, JsonPatchDocument<FieldForUpdateDto> patchDocument);
        #endregion

        #region  FieldObservation
        Task<GenericResponse<FieldObservationDto>> AddNewFieldObservation(FieldObservationForCreationDto fieldObservationForCreationDto, HttpContext httpContext, string mediaType);
        Task<GenericResponse> DeleteFieldObservation(Guid id, HttpContext httpContext);
        Task<GenericResponse<ShapedDataWithLinks>> GetFieldObservations(Guid fieldId, FieldObservationResourceParameter resourceParameter, string mediaType);
        GenericResponse<FieldObservationDto> GetFieldObservationDto(Guid id, string fields, string mediaType, HttpContext httpContext);
        #endregion

        #region  FieldSpray
        Task<GenericResponse<FieldSprayApplicationDto>> AddNewFieldSpray(FieldSprayApplicationForCreationDto sprayForCreationDto, HttpContext httpContext, string mediaType);
        Task<GenericResponse> DeleteFieldSpray(Guid id, HttpContext httpContext);
        GenericResponse<FieldSprayApplicationDto> GetFieldSprayDto(Guid id, string mediaType, HttpContext httpContext);
        Task<GenericResponse<ShapedDataWithLinks>> GetFieldSprays(Guid fieldId, FieldSprayResourceParameter resourceParameter, string mediaType);
        #endregion

        #region  UserProfile
        Task<GenericResponse<IDictionary<string, object>>> AddNewUserProfile(Guid userId, UserProfileForCreationDto userProfileForCreation, string mediaType);
        Task<GenericResponse<UserProfileDto>> AddNewUserProfile(Guid userId, UserProfileForCreationDto userProfileForCreation);
        Task<GenericResponse> DeleteUserProfileClient(Guid userId);
        Task<GenericResponse<UserProfile>> GetUserProfileByUserId(Guid userId, bool includeAssociatedData = false);
        Task<GenericResponse<IDictionary<string, object>>> GetUserProfileDto(Guid userId, string fields, string mediaType);
        UserProfileForCreationDto MapToUserProfileForCreation(UserProfileForUpdateDto userProfileDto);
        UserProfileForUpdateDto MapToUserProfileForUpdateDto(UserProfile userProfile);
        Task<GenericResponse> UpdateUserProfile(UserProfile userProfile, UserProfileForUpdateDto userProfileToPatch);
        #endregion

        #region Data Sharing
        Task<GenericResponse> DeleteDataShareRequest(Guid id, Guid userId);
        Task<GenericResponse<bool>> AddDataShareRequest(Guid userId, DataShareRequestForCreationDto dataShareRequestDto);
        Task<GenericResponse<ShapedDataWithLinks>> GetDataShareRequests(Guid userId, DataShareResourceParameter resourceParameter);
        Task<GenericResponse> ReplyToDataShareRequest(Guid userId, DataShareRequestReplyDto dataShareRequestDto);
        Task<GenericResponse> UpdateDataShareRequest(Guid userId, DataShareRequestUpdateDto dataShareRequestDto);
        #endregion

        #region Internal Call
        Task<GenericResponse> InitialUserProfileCreation(UserProfileInternalCallDto userProfileDto);
        #endregion

        #region  UserWidget
        Task<GenericResponse<IEnumerable<UserWidgetDto>>> GetUserWidgets(Guid userId);
        Task<GenericResponse> UpdateUserWidgets(Guid userId, JsonPatchDocument<UserWidgetForUpdateDto> patchDocument);
        #endregion

        #region User DSS
        Task<GenericResponse<List<FieldCropPestDssDto>>> GetAllUserFieldCropPestDss(Guid userId);
        Task<GenericResponse<FieldCropPestDssDto>> GetFieldCropPestDssById(Guid id, Guid userId);
        #endregion

        #region DssResults
        Task<GenericResponse<FieldDssResultDto>> GetLatestFieldCropPestDssResult(Guid dssId, Guid userId);
        Task<GenericResponse<FieldDssResultDto>> CreateFieldCropPestDssResult(Guid dssId, Guid userId, FieldDssResultForCreationDto dssResultDto);
        #endregion
    }
}