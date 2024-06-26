using H2020.IPMDecisions.UPR.Core.Dtos;
using H2020.IPMDecisions.UPR.Core.Entities;
using H2020.IPMDecisions.UPR.Core.Enums;
using H2020.IPMDecisions.UPR.Core.Models;
using H2020.IPMDecisions.UPR.Core.ResourceParameters;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace H2020.IPMDecisions.UPR.BLL
{
    public interface IBusinessLogic
    {
        #region User Farms
        Task<GenericResponse<IDictionary<string, object>>> LinkNewFarmToUserProfile(FarmForCreationDto farmForCreationDto, Guid userId, string mediaType);
        #endregion

        #region Crop Decision
        Task<GenericResponse<IDictionary<string, object>>> AddNewFieldCropDecision(CropPestDssForCreationDto cropPestDssForCreationDto, HttpContext httpContext, string mediaType);
        Task<GenericResponse> DeleteFieldCropDecision(Guid id, HttpContext httpContext);
        Task<GenericResponse<IDictionary<string, object>>> GetFieldCropDecision(Guid id, HttpContext httpContext, string mediaType);
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
        Task<GenericResponse> FullUpdateFarm(Farm farm, FarmForFullUpdateDto farmForFullUpdate);
        #endregion

        #region FarmDss
        Task<GenericResponse<IDictionary<string, object>>> AddListOfFarmDss(IEnumerable<FarmDssForCreationDto> listOfFarmDssDto, HttpContext httpContext, string mediaType);
        Task<GenericResponse<IDictionary<string, object>>> AddListOfLinkDssToFarm(IEnumerable<FarmDssForCreationDto> farmDssDto, HttpContext httpContext, string mediaType);
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
        Task<GenericResponse> DeleteUserProfile(Guid userId);
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
        Task<bool> UserHasAnyDss(Guid userId);
        Task<List<ReportData>> GetDataForReport();
        #endregion

        #region  UserWidget
        Task<GenericResponse<IEnumerable<UserWidgetDto>>> GetUserWidgets(Guid userId);
        Task<GenericResponse> UpdateUserWidgets(Guid userId, JsonPatchDocument<UserWidgetForUpdateDto> patchDocument);
        #endregion

        #region User DSS
        Task<GenericResponse<FieldDssResultDetailedDto>> GetFieldCropPestDssById(Guid id, Guid userId, int daysDataToReturn);
        Task<GenericResponse<JObject>> GetFieldCropPestDssParametersById(Guid id, Guid userId, bool? displayInternalParameters);
        Task<GenericResponse<JObject>> GetFieldCropPestDssDefaultParametersById(Guid id, Guid userId);
        Task<GenericResponse> UpdateFieldCropPestDssById(Guid id, Guid userId, FieldCropPestDssForUpdateDto fieldCropPestDssForUpdateDto);
        Task<GenericResponse> DeleteDss(Guid id, Guid userId);
        Task<GenericResponse<IEnumerable<FieldDssResultDto>>> GetAllDssResults(Guid userId, DssResultListFilterDto filterDto);
        Task<GenericResponse<IEnumerable<LinkDssDto>>> GetAllLinkDss(Guid userId);
        Task<GenericResponse<IEnumerable<DssInformation>>> GetAllAvailableDssOnFarmLocation(DssListFilterDto dssListFilterDto, Guid userId);
        Task<GenericResponse<byte[]>> GetFieldCropPestDssDataAsCSVById(Guid id, Guid userId);
        #endregion

        #region DssResults
        Task<GenericResponse<FieldDssResultDto>> GetLatestFieldCropPestDssResult(Guid dssId, Guid userId);
        Task<GenericResponse<FieldDssResultDto>> CreateFieldCropPestDssResult(Guid dssId, Guid userId, FieldDssResultForCreationDto dssResultDto);
        #endregion

        #region EppoCodes
        Task<GenericResponse<EppoCodeTypeDto>> CreateEppoCodeType(EppoCodeForCreationDto eppoCodeForCreationDto);
        Task<GenericResponse<List<string>>> GetEppoCodeTypes();
        Task<GenericResponse<List<EppoCodeTypeDto>>> GetAllEppoCodes();
        Task<GenericResponse<EppoCodeTypeDto>> GetEppoCode(string eppoCodeType, string eppoCode, string executionType);
        Task<GenericResponse> UpdateEppoCodeType(string eppoCodeType, EppoCodeForUpdateDto eppoCodeForUpdateDto);
        #endregion

        #region Administration
        Task<GenericResponse<IEnumerable<AdminVariableDto>>> GetAllAdminVariables();
        Task<GenericResponse> UpdateAdminVariableById(AdminValuesEnum id, AdminVariableForManipulationDto adminVariableForManipulationDto);
        Task<GenericResponse> RemoveDisabledDssFromListAsync(List<Guid> ids);
        Task<GenericResponse<IEnumerable<DisabledDssDto>>> GetAllDisabledDss();
        Task<GenericResponse<IEnumerable<DisabledDssDto>>> AddDisabledDssFromListAsync(IEnumerable<DisabledDssForCreationDto> listOfDisabledDssDto);
        #endregion

        #region Comparison DSS
        Task<GenericResponse<IEnumerable<FieldDssResultDetailedDto>>> CompareDssByIds(List<Guid> ids, Guid userId, int daysDataToReturn);
        #endregion

        #region Adaptation DSS
        Task<GenericResponse<AdaptationDashboardDto>> GetAdaptationDataById(Guid id, Guid userId, int daysDataToReturn);
        Task<GenericResponse<DssTaskStatusDto>> AddTaskToRunFieldCropPestDssById(Guid id, Guid userId, FieldCropPestDssForUpdateDto fieldCropPestDssForUpdateDto);
        Task<GenericResponse<AdaptationTaskDssResult>> GetDssResultFromTaskById(Guid dssId, string taskId, Guid userId, int daysDataToReturn);
        Task<GenericResponse<List<DssHistoricalDataTask>>> PrepareDssToRunFieldCropPestDssHistoricalDataById(Guid id, Guid userId, FieldCropPestDssForHistoricalDataDto fieldCropPestDssForUpdateDto);
        Task<GenericResponse<IDictionary<string, object>>> SaveAdaptationDss(Guid id, Guid userId, FieldCropPestDssForAdaptationDto fieldCropPestDssForAdaptationDto, HttpContext httpContext);
        #endregion

        #region DSS Result Task
        Task<GenericResponse<DssTaskStatusDto>> GetLatestTaskStatusByDssId(Guid dssId, Guid userId);
        #endregion

        #region Weather
        Task<GenericResponse<List<WeatherBaseDto>>> GetWeatherDataSources();
        Task<GenericResponse<UserWeatherDto>> GetUserWeatherById(Guid id, Guid userId);
        Task<GenericResponse<IEnumerable<UserWeatherDto>>> GetUserWeathers(Guid userId);
        Task<GenericResponse<UserWeatherDto>> CreateUserWeather(Guid userId, UserWeatherForCreationDto userWeatherForCreationDto);
        Task<GenericResponse> RemoveUserWeather(Guid id, Guid userId);
        Task<GenericResponse> UpdateUserWeatherById(Guid id, Guid userId, UserWeatherForUpdateDto userWeatherForUpdateDto);
        Task<GenericResponse> RemoveUserWeatherToFarms(Guid id, Guid userId, List<Guid> farmIds);
        Task<GenericResponse> AddUserWeatherToFarms(Guid id, Guid userId, List<Guid> farmIds);
        #endregion

        #region RiskMaps
        Task<GenericResponse<List<RiskMapBaseDto>>> GetRiskMapDataSources();
        Task<GenericResponse<RiskMapFullDetailDto>> GetRiskMapFilteredDataSource(string providerId, string id);
        #endregion
    }
}