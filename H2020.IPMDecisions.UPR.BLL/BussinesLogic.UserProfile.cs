using System;
using System.Collections.Generic;
using Microsoft.Net.Http.Headers;
using System.Threading.Tasks;
using H2020.IPMDecisions.UPR.Core.Dtos;
using H2020.IPMDecisions.UPR.Core.Entities;
using H2020.IPMDecisions.UPR.Core.Models;
using H2020.IPMDecisions.UPR.Core.Helpers;
using H2020.IPMDecisions.UPR.BLL.Helpers;

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
                    return GenericResponseBuilder.NoSuccess<IDictionary<string, object>>(null, "Wrong media type.");

                var currentUserProfileExists = await GetUserProfile(userId);
                if (currentUserProfileExists.Result != null)
                {
                    return GenericResponseBuilder.NoSuccess<IDictionary<string, object>>(null, "User profile aready exits. Please use `PATCH` method for partial updates.");
                }
                    

                var userProfileEntity = this.mapper.Map<UserProfile>(userProfileForCreation);
                userProfileEntity.UserId = userId;

                this.dataService.UserProfiles.Create(userProfileEntity);

                await this.dataService.CompleteAsync();

                var userProfileToReturn = this.mapper.Map<UserProfileDto>(userProfileEntity)
                    .ShapeData()
                    as IDictionary<string, object>;

                var includeLinks = parsedMediaType.SubTypeWithoutSuffix
                            .EndsWith("hateoas", StringComparison.InvariantCultureIgnoreCase);
                if (includeLinks)
                {
                    var links = UrlCreatorHelper.CreateLinksForUserProfiles(this.url, userId);
                    userProfileToReturn.Add("links", links);
                }

                return GenericResponseBuilder.Success<IDictionary<string, object>>(userProfileToReturn);
            }
            catch (Exception ex)
            {
                //ToDo Log Error
                return GenericResponseBuilder.NoSuccess<IDictionary<string, object>>(null, $"{ex.Message} InnerException: {ex.InnerException.Message}");
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
                //ToDo Log Error
                return GenericResponseBuilder.NoSuccess<UserProfileDto>(null, $"{ex.Message} InnerException: {ex.InnerException.Message}");
            }
        }

        public async Task<GenericResponse> DeleteUserProfileClient(Guid userId)
        {
            try
            {
                var existingUserProfile = await this.dataService
                .UserProfiles
                .FindByCondition(u => u.UserId == userId);

                if (existingUserProfile == null) return GenericResponseBuilder.Success();

                this.dataService.UserProfiles.Delete(existingUserProfile);
                await this.dataService.CompleteAsync();

                return GenericResponseBuilder.Success();
            }
            catch (Exception ex)
            {
                // ToDo Log Error
                return GenericResponseBuilder.NoSuccess($"{ex.Message} InnerException: {ex.InnerException.Message}");
            }
        }

        public async Task<GenericResponse<UserProfile>> GetUserProfile(Guid userId)
        {
            try
            {
                var existingUserProfile = await this.dataService
                    .UserProfiles
                    .FindByCondition(u => u.UserId == userId);

                if (existingUserProfile == null) return GenericResponseBuilder.Success<UserProfile>(null);

                return GenericResponseBuilder.Success<UserProfile>(existingUserProfile);
            }
            catch (Exception ex)
            {
                //ToDo Log Error
                return GenericResponseBuilder.NoSuccess<UserProfile>(null, $"{ex.Message} InnerException: {ex.InnerException.Message}");
            }
        }

        public async Task<GenericResponse<IDictionary<string, object>>> GetUserProfileDto(Guid userId, string fields, string mediaType)
        {
            try
            {
                if (!MediaTypeHeaderValue.TryParse(mediaType,
                       out MediaTypeHeaderValue parsedMediaType))
                    return GenericResponseBuilder.NoSuccess<IDictionary<string, object>>(null, "Wrong media type");

                if (!propertyCheckerService.TypeHasProperties<UserProfileDto>(fields))
                    return GenericResponseBuilder.NoSuccess<IDictionary<string, object>>(null, "Wrong fields entered");

                var existingUserProfile = await this.dataService
                    .UserProfiles
                    .FindByCondition(u => u.UserId == userId);

                if (existingUserProfile == null) return GenericResponseBuilder.Success<IDictionary<string, object>>(null);                

                var userProfileToReturn = this.mapper.Map<UserProfileDto>(existingUserProfile)
                    .ShapeData(fields) 
                    as IDictionary<string, object>;

                var includeLinks = parsedMediaType.SubTypeWithoutSuffix
                            .EndsWith("hateoas", StringComparison.InvariantCultureIgnoreCase);
                if (includeLinks)
                {
                    var links = UrlCreatorHelper.CreateLinksForUserProfiles(this.url, userId);
                    userProfileToReturn.Add("links", links);
                }
                return GenericResponseBuilder.Success<IDictionary<string, object>>(userProfileToReturn);
            }
            catch (Exception ex)
            {
                //ToDo Log Error
                return GenericResponseBuilder.NoSuccess<IDictionary<string, object>>(null, $"{ex.Message} InnerException: {ex.InnerException.Message}");
            }
        }

        public UserProfileForCreationDto MapToUserProfileForCreation(UserProfileForUpdateDto userProfileDto)
        {
            return this.mapper.Map<UserProfileForCreationDto>(userProfileDto);
        }

        public UserProfileForUpdateDto MapToUserProfileForUpdateDto(UserProfile userProfile)
        {
            return this.mapper.Map<UserProfileForUpdateDto>(userProfile);
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
                //ToDo Log Error
                return GenericResponseBuilder.NoSuccess($"{ex.Message} InnerException: {ex.InnerException.Message}");
            }
        }

        #region Helpers        
        #endregion
    }
}