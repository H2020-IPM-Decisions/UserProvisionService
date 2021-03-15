using System;
using System.Threading.Tasks;
using H2020.IPMDecisions.UPR.Core.Dtos;
using H2020.IPMDecisions.UPR.Core.Entities;
using H2020.IPMDecisions.UPR.Core.Models;
using Microsoft.Extensions.Logging;

namespace H2020.IPMDecisions.UPR.BLL
{
    public partial class BusinessLogic : IBusinessLogic
    {

        public async Task<GenericResponse> InitialUserProfileCreation(UserProfileInternalCallDto userProfileDto)
        {
            try
            {
                var userProfileEntity = this.mapper.Map<UserProfile>(userProfileDto);
                userProfileEntity.UserId = userProfileDto.UserId;

                this.dataService.UserProfiles.Create(userProfileEntity);
                await this.dataService.UserWidgets.InitialCreation(userProfileDto.UserId);
                await this.dataService.CompleteAsync();

                this.mapper.Map<UserProfileDto>(userProfileEntity);
                return GenericResponseBuilder.Success();
            }
            catch (Exception ex)
            {
                logger.LogError(string.Format("Error in BLL - AddNewUserProfile. {0}", ex.Message), ex);
                String innerMessage = (ex.InnerException != null) ? ex.InnerException.Message : "";
                return GenericResponseBuilder.NoSuccess($"{ex.Message} InnerException: {innerMessage}");
            }
        }


        #region Helpers

        #endregion
    }
}