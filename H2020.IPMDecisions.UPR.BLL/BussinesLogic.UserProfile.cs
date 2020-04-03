using System;
using System.Threading.Tasks;
using H2020.IPMDecisions.UPR.Core.Dtos;
using H2020.IPMDecisions.UPR.Core.Entities;
using H2020.IPMDecisions.UPR.Core.Models;

namespace H2020.IPMDecisions.UPR.BLL
{
    public partial class BusinessLogic : IBusinessLogic
    {
        public async Task<GenericResponse> AddNewUserProfile(Guid userId, UserProfileForCreationDto userProfileForCreation)
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
                return GenericResponseBuilder.NoSuccess($"{ex.Message} InnerException: {ex.InnerException.Message}");
            }
        }
    }
}