using System;
using System.Collections.Generic;
using Microsoft.Net.Http.Headers;
using System.Threading.Tasks;
using H2020.IPMDecisions.UPR.Core.Dtos;
using H2020.IPMDecisions.UPR.Core.Entities;
using H2020.IPMDecisions.UPR.Core.Models;
using H2020.IPMDecisions.UPR.Core.Helpers;
using H2020.IPMDecisions.UPR.BLL.Helpers;
using Microsoft.Extensions.Logging;

namespace H2020.IPMDecisions.UPR.BLL
{
    public partial class BusinessLogic : IBusinessLogic
    {
        public async Task<GenericResponse<IDictionary<string, object>>> AddNewUserProfile(Guid userId, UserProfileForCreationDto userProfileForCreation, string mediaType)
        {
            try
            {
                if (!MediaTypeHeaderValue.TryParse(mediaType,
                       out MediaTypeHeaderValue parsedMediaType))
                    return GenericResponseBuilder.NoSuccess<IDictionary<string, object>>(null, this.jsonStringLocalizer["shared.wrong_media_type"].ToString());

                var currentUserProfileExists = await GetUserProfileByUserId(userId);
                if (currentUserProfileExists.Result != null)
                {
                    return GenericResponseBuilder.NoSuccess<IDictionary<string, object>>(null, "User profile already exits. Please use `PATCH` method for partial updates.");
                }

                var userProfileEntity = this.mapper.Map<UserProfile>(userProfileForCreation);
                userProfileEntity.UserId = userId;

                this.dataService.UserProfiles.Create(userProfileEntity);
                await this.dataService.CompleteAsync();

                var includeLinks = parsedMediaType.SubTypeWithoutSuffix
                            .EndsWith("hateoas", StringComparison.InvariantCultureIgnoreCase);

                var primaryMediaType = includeLinks ?
                    parsedMediaType.SubTypeWithoutSuffix
                    .Substring(0, parsedMediaType.SubTypeWithoutSuffix.Length - 8)
                    : parsedMediaType.SubTypeWithoutSuffix;

                if (primaryMediaType == "vnd.h2020ipmdecisions.profile.full")
                {
                    var userProfileFullToReturn = this.mapper.Map<UserProfileFullDto>(userProfileEntity)
                        .ShapeData()
                        as IDictionary<string, object>;

                    AddLinksToUserProfileAsDictionary(includeLinks, userProfileFullToReturn);
                    return GenericResponseBuilder.Success<IDictionary<string, object>>(userProfileFullToReturn);
                }

                var userProfileToReturn = this.mapper.Map<UserProfileDto>(userProfileEntity)
                    .ShapeData()
                    as IDictionary<string, object>;

                AddLinksToUserProfileAsDictionary(includeLinks, userProfileToReturn);

                return GenericResponseBuilder.Success<IDictionary<string, object>>(userProfileToReturn);
            }
            catch (Exception ex)
            {
                logger.LogError(string.Format("Error in BLL - AddNewUserProfile. {0}", ex.Message), ex);
                String innerMessage = (ex.InnerException != null) ? ex.InnerException.Message : "";
                return GenericResponseBuilder.NoSuccess<IDictionary<string, object>>(null, $"{ex.Message} InnerException: {innerMessage}");
            }
        }      

        public async Task<GenericResponse<UserProfileDto>> AddNewUserProfile(Guid userId, UserProfileForCreationDto userProfileForCreation)
        {
            try
            {
                var userProfileEntity = this.mapper.Map<UserProfile>(userProfileForCreation);
                userProfileEntity.UserId = userId;

                this.dataService.UserProfiles.Create(userProfileEntity);
                await this.dataService.CompleteAsync();

                var userToReturn = this.mapper.Map<UserProfileDto>(userProfileEntity);
                return GenericResponseBuilder.Success<UserProfileDto>(userToReturn);
            }
            catch (Exception ex)
            {
                logger.LogError(string.Format("Error in BLL - AddNewUserProfile. {0}", ex.Message), ex);
                String innerMessage = (ex.InnerException != null) ? ex.InnerException.Message : "";
                return GenericResponseBuilder.NoSuccess<UserProfileDto>(null, $"{ex.Message} InnerException: {innerMessage}");
            }
        }

        public async Task<GenericResponse> DeleteUserProfile(Guid userId)
        {
            try
            {
                var existingUserProfile = await this.dataService
                    .UserProfiles
                    .FindByConditionAsync(u => u.UserId == userId);

                if (existingUserProfile == null) return GenericResponseBuilder.Success();

                this.dataService.UserProfiles.Delete(existingUserProfile);
                await this.dataService.CompleteAsync();

                return GenericResponseBuilder.Success();
            }
            catch (Exception ex)
            {
                logger.LogError(string.Format("Error in BLL - DeleteUserProfileClient. {0}", ex.Message), ex);
                String innerMessage = (ex.InnerException != null) ? ex.InnerException.Message : "";
                return GenericResponseBuilder.NoSuccess($"{ex.Message} InnerException: {innerMessage}");
            }
        }

        public async Task<GenericResponse<UserProfile>> GetUserProfileByUserId(Guid userId, bool includeAssociatedData = false)
        {
            try
            {
                var existingUserProfile = await this.dataService
                    .UserProfiles
                    .FindByConditionAsync(u => u.UserId == userId, includeAssociatedData);

                if (existingUserProfile == null) return GenericResponseBuilder.Success<UserProfile>(null);

                return GenericResponseBuilder.Success<UserProfile>(existingUserProfile);
            }
            catch (Exception ex)
            {
                logger.LogError(string.Format("Error in BLL - GetUserProfile. {0}", ex.Message), ex);
                String innerMessage = (ex.InnerException != null) ? ex.InnerException.Message : "";
                return GenericResponseBuilder.NoSuccess<UserProfile>(null, $"{ex.Message} InnerException: {innerMessage}");
            }
        }

        public async Task<GenericResponse<IDictionary<string, object>>> GetUserProfileDto(Guid userId, string fields, string mediaType)
        {
            try
            {
                if (!MediaTypeHeaderValue.TryParse(mediaType,
                       out MediaTypeHeaderValue parsedMediaType))
                    return GenericResponseBuilder.NoSuccess<IDictionary<string, object>>(null, this.jsonStringLocalizer["shared.wrong_media_type"].ToString());

                if (!propertyCheckerService.TypeHasProperties<UserProfileDto>(fields))
                    return GenericResponseBuilder.NoSuccess<IDictionary<string, object>>(null, "Wrong fields entered");

                var existingUserProfile = await this.dataService
                    .UserProfiles
                    .FindByConditionAsync(u => u.UserId == userId);

                if (existingUserProfile == null) return GenericResponseBuilder.Success<IDictionary<string, object>>(null);

                var includeLinks = parsedMediaType.SubTypeWithoutSuffix
                            .EndsWith("hateoas", StringComparison.InvariantCultureIgnoreCase);

                var primaryMediaType = includeLinks ?
                    parsedMediaType.SubTypeWithoutSuffix
                    .Substring(0, parsedMediaType.SubTypeWithoutSuffix.Length - 8)
                    : parsedMediaType.SubTypeWithoutSuffix;

                if (primaryMediaType == "vnd.h2020ipmdecisions.profile.full")
                {
                    var userProfileFullToReturn = this.mapper.Map<UserProfileFullDto>(existingUserProfile)
                        .ShapeData(fields)
                        as IDictionary<string, object>;

                    AddLinksToUserProfileAsDictionary(includeLinks, userProfileFullToReturn);
                    return GenericResponseBuilder.Success<IDictionary<string, object>>(userProfileFullToReturn);
                }
                
                var userProfileToReturn = this.mapper.Map<UserProfileDto>(existingUserProfile)
                        .ShapeData(fields)
                        as IDictionary<string, object>;

                AddLinksToUserProfileAsDictionary(includeLinks, userProfileToReturn);
                return GenericResponseBuilder.Success<IDictionary<string, object>>(userProfileToReturn);
            }
            catch (Exception ex)
            {
                logger.LogError(string.Format("Error in BLL - GetUserProfileDto. {0}", ex.Message), ex);
                String innerMessage = (ex.InnerException != null) ? ex.InnerException.Message : "";
                return GenericResponseBuilder.NoSuccess<IDictionary<string, object>>(null, $"{ex.Message} InnerException: {innerMessage}");
            }
        }

        public async Task<GenericResponse> UpdateUserProfile(UserProfile userProfile, UserProfileForUpdateDto userProfileToPatch)
        {
            try
            {
                this.mapper.Map(userProfileToPatch, userProfile);

                this.dataService.UserProfiles.Update(userProfile);
                await this.dataService.CompleteAsync();

                return GenericResponseBuilder.Success();
            }
            catch (Exception ex)
            {
                logger.LogError(string.Format("Error in BLL - UpdateUserProfile. {0}", ex.Message), ex);
                String innerMessage = (ex.InnerException != null) ? ex.InnerException.Message : "";
                return GenericResponseBuilder.NoSuccess($"{ex.Message} InnerException: {innerMessage}");
            }
        }

        #region Helpers
        public UserProfileForCreationDto MapToUserProfileForCreation(UserProfileForUpdateDto userProfileDto)
        {
            return this.mapper.Map<UserProfileForCreationDto>(userProfileDto);
        }

        public UserProfileForUpdateDto MapToUserProfileForUpdateDto(UserProfile userProfile)
        {
            return this.mapper.Map<UserProfileForUpdateDto>(userProfile);
        }

        private void AddLinksToUserProfileAsDictionary(bool includeLinks, IDictionary<string, object> userProfileAsDictionary)
        {
            if (includeLinks)
            {
                var links = UrlCreatorHelper.CreateLinksForUserProfile(this.url);
                userProfileAsDictionary.Add("links", links);
            }
        }
        #endregion
    }
}