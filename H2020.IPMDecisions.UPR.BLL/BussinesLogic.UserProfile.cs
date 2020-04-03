using System;
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

        public async Task<GenericResponse<UserProfileDto>> GetUserProfile(Guid userId)
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
    }
}