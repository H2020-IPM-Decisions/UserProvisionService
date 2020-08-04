using H2020.IPMDecisions.UPR.Core.Dtos;
using H2020.IPMDecisions.UPR.Core.Enums;
using H2020.IPMDecisions.UPR.Core.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace H2020.IPMDecisions.UPR.BLL
{
    public partial class BusinessLogic : IBusinessLogic
    {
        public async Task<GenericResponse<bool>> RequestDataShare(Guid userId, DataShareRequestDto dataShareRequestDto, string mediaType)
        {
            try
            {
                if (!MediaTypeHeaderValue.TryParse(mediaType,
                       out MediaTypeHeaderValue parsedMediaType))
                    return GenericResponseBuilder.NoSuccess<bool>(false, "Wrong media type.");
                
                var requesteeUserId = await this.internalCommunicationProvider.GetUserIdFromIdpMicroservice(dataShareRequestDto.Email.ToString());
                if (string.IsNullOrEmpty(requesteeUserId))
                {
                    return GenericResponseBuilder.NoSuccess<bool>(false, "User requested not in the system.");
                }

                var requesteeUserProfileExists = await GetUserProfile(Guid.Parse(requesteeUserId));
                if (requesteeUserProfileExists.Result == null)
                {
                    return GenericResponseBuilder.NoSuccess<bool>(false, "User requested do not have a profile in the system.");
                }

                var requesterUserProfileExists = await GetUserProfile(userId);
                if (requesterUserProfileExists.Result == null)
                {
                    return GenericResponseBuilder.NoSuccess<bool>(false, "User requesting data share do not have a profile in the system.");
                }

                var requestExist = await this.dataService.DataShareRequests.FindByCondition(
                        d => (d.RequesterId == requesterUserProfileExists.Result.Id) 
                        && (d.RequesteeId == requesteeUserProfileExists.Result.Id), true);

                if (requestExist != null 
                    && (
                        requestExist.RequestStatus.Description.Equals(RequestStatusEnum.Revoked.ToString())
                        || requestExist.RequestStatus.Description.Equals(RequestStatusEnum.Declined.ToString())
                    ))
                {
                    return GenericResponseBuilder.NoSuccess<bool>(false, "The user has declined or revoked access to the data.");
                }
                else if (requestExist != null)
                {
                    return GenericResponseBuilder.NoSuccess<bool>(false, "You have already requested data to this user.");
                }

                await this.dataService.DataShareRequests.Create(
                    requesterUserProfileExists.Result.Id,
                    requesteeUserProfileExists.Result.Id,
                    RequestStatusEnum.Pending);
                await this.dataService.CompleteAsync();

                var requesterFullName = string.Format(
                    "{0} {1}", 
                    requesterUserProfileExists.Result.FirstName, 
                    requesterUserProfileExists.Result.LastName)
                    .Trim();

                var emailSent = await this.internalCommunicationProvider.SendDataRequestEmail(
                    requesterFullName,
                    dataShareRequestDto.Email.ToString());
                
                if (!emailSent)
                {
                    return GenericResponseBuilder.NoSuccess<bool>(false, "Request created but error sending email.");
                }

                return GenericResponseBuilder.Success<bool>(true);
            }
            catch (Exception ex)
            {
                logger.LogError(string.Format("Error in BLL - RequestDataShare. {0}", ex.Message));
                return GenericResponseBuilder.NoSuccess<bool>(false, ex.Message.ToString());
            }
        }

        #region Helpers
        
        #endregion
    }
}