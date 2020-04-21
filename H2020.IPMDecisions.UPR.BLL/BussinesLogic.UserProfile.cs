using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using H2020.IPMDecisions.UPR.Core.Dtos;
using H2020.IPMDecisions.UPR.Core.Entities;
using H2020.IPMDecisions.UPR.Core.Models;

namespace H2020.IPMDecisions.UPR.BLL
{
    public partial class BusinessLogic : IBusinessLogic
    {
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

        public async Task<GenericResponse<UserProfileDto>> GetUserProfileDto(Guid userId)
        {
            try
            {
                var existingUserProfile = await this.dataService
                    .UserProfiles
                    .FindByCondition(u => u.UserId == userId);

                if (existingUserProfile == null) return GenericResponseBuilder.Success<UserProfileDto>(null);

                var userToReturn = this.mapper.Map<UserProfileDto>(existingUserProfile);
                return GenericResponseBuilder.Success<UserProfileDto>(userToReturn);
            }
            catch (Exception ex)
            {
                //ToDo Log Error
                return GenericResponseBuilder.NoSuccess<UserProfileDto>(null, $"{ex.Message} InnerException: {ex.InnerException.Message}");
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
        private IEnumerable<LinkDto> CreateLinksForUserProfiles(
           Guid userId,
           string fields = "")
        {
            var links = new List<LinkDto>();

            if (string.IsNullOrWhiteSpace(fields))
            {
                links.Add(new LinkDto(
                url.Link("GetUserProfile", new { userId }),
                "self",
                "GET"));
            }
            else
            {
                links.Add(new LinkDto(
                 url.Link("GetUserProfile", new { userId, fields }),
                 "self",
                 "GET"));
            }

            links.Add(new LinkDto(
                url.Link("DeleteUserProfile", new { userId }),
                "delete_user_profile",
                "DELETE"));

            links.Add(new LinkDto(
                url.Link("PartialUpdateUserProfile", new { userId }),
                "update_user_profile",
                "PATCH"));

            return links;
        }
        #endregion
    }
}