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
        public async Task<GenericResponse> RequestDataShare(Guid userId, DataShareRequestDto dataShareRequestDto, string mediaType)
        {
            try
            {
                if (!MediaTypeHeaderValue.TryParse(mediaType,
                       out MediaTypeHeaderValue parsedMediaType))
                    return GenericResponseBuilder.NoSuccess("Wrong media type.");
                
                var requesteeUserId = await this.internalCommunicationProvider.GetUserIdFromIdpMicroservice(dataShareRequestDto.Email.ToString());
                if (string.IsNullOrEmpty(requesteeUserId))
                {
                    return GenericResponseBuilder.NoSuccess("User requested not in the system.");
                }
                var requesteeUserIdAsGuid = Guid.Parse(requesteeUserId);
                var requesteeUserProfileExists = await GetUserProfile(requesteeUserIdAsGuid);
                if (requesteeUserProfileExists.Result == null)
                {
                    return GenericResponseBuilder.NoSuccess("User requested do not have a profile in the system.");
                }

                var requesterUserProfileExists = await GetUserProfile(userId);
                if (requesterUserProfileExists.Result == null)
                {
                    return GenericResponseBuilder.NoSuccess("User requesting data share do not have a profile in the system.");
                }                
                var requestExist = await this.dataService.DataShareRequests.FindByCondition(
                        d => (d.RequesterId == requesterUserProfileExists.Result.Id) 
                        && (d.RequesteeId == requesteeUserProfileExists.Result.Id)
                        && (d.RequestStatus.Description == RequestStatusEnum.Pending.ToString()));
                
                if (requestExist != null)
                {
                    return GenericResponseBuilder.NoSuccess("Already requested data from this user.");
                }
                await this.dataService.DataShareRequests.Create(
                    requesterUserProfileExists.Result.Id,
                    requesteeUserProfileExists.Result.Id,
                    RequestStatusEnum.Pending);

                await this.dataService.CompleteAsync();

                // ToDo: Send email with request

                return GenericResponseBuilder.Success();
            }
            catch (Exception ex)
            {
                logger.LogError(string.Format("Error in BLL - RequestDataShare. {0}", ex.Message));
                return GenericResponseBuilder.NoSuccess(ex.Message.ToString());
            }
        }

        #region Helpers
        
        #endregion
    }
}