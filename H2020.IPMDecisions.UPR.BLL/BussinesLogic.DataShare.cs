using H2020.IPMDecisions.UPR.BLL.Helpers;
using H2020.IPMDecisions.UPR.Core.Dtos;
using H2020.IPMDecisions.UPR.Core.Entities;
using H2020.IPMDecisions.UPR.Core.Enums;
using H2020.IPMDecisions.UPR.Core.Helpers;
using H2020.IPMDecisions.UPR.Core.Models;
using H2020.IPMDecisions.UPR.Core.ResourceParameters;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace H2020.IPMDecisions.UPR.BLL
{
    public partial class BusinessLogic : IBusinessLogic
    {
        public async Task<GenericResponse<bool>> AddRequestDataShare(Guid userId, DataShareRequestForCreationDto dataShareRequestDto)
        {
            try
            {
                var requesteeUserId = await this.internalCommunicationProvider.GetUserIdFromIdpMicroservice(dataShareRequestDto.Email.ToString());
                if (string.IsNullOrEmpty(requesteeUserId))
                {
                    return GenericResponseBuilder.NoSuccess<bool>(false, "User requested not in the system.");
                }

                var requesteeUserProfileExists = await GetUserProfileByUserId(Guid.Parse(requesteeUserId));
                if (requesteeUserProfileExists.Result == null)
                {
                    return GenericResponseBuilder.NoSuccess<bool>(false, "User requested do not have a profile in the system.");
                }

                var requesterUserProfileExists = await GetUserProfileByUserId(userId);
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
                String innerMessage = (ex.InnerException != null) ? ex.InnerException.Message : "";
                return GenericResponseBuilder.NoSuccess<bool>(false, $"{ex.Message} InnerException: {innerMessage}");
            }
        }

        public async Task<GenericResponse<ShapedDataWithLinks>> GetDataShareRequests(Guid userId, DataShareResourceParameter resourceParameter)
        {
            try
            {
                if (!propertyMappingService.ValidMappingExistsFor<DataShareRequestDto, DataSharingRequest>(resourceParameter.OrderBy))
                    return GenericResponseBuilder.NoSuccess<ShapedDataWithLinks>(null, "Wrong OrderBy entered");

                var userProfile = await GetUserProfileByUserId(userId);
                if (userProfile.Result == null)
                    return GenericResponseBuilder.NoSuccess<ShapedDataWithLinks>(null, "Please create an `User Profile` first.");

                var requestsAsEntities = await this
                    .dataService
                    .DataShareRequests
                    .FindAllAsync(userProfile.Result.Id, resourceParameter);

                if (requestsAsEntities.Count == 0) return GenericResponseBuilder.NotFound<ShapedDataWithLinks>();

                var paginationMetaData = MiscellaneousHelper.CreatePaginationMetadata(requestsAsEntities);
                var paginationLinks = UrlCreatorHelper.CreateLinksForRequests(
                    url,
                    resourceParameter,
                    requestsAsEntities.HasNext,
                    requestsAsEntities.HasPrevious);

                var shapedRequestsToReturn = this.mapper
                   .Map<IEnumerable<DataShareRequestDto>>(requestsAsEntities)
                   .ShapeData();

                var requestsToReturn = new ShapedDataWithLinks()
                {
                    Value = shapedRequestsToReturn,
                    Links = paginationLinks,
                    PaginationMetaData = paginationMetaData
                };

                return GenericResponseBuilder.Success<ShapedDataWithLinks>(requestsToReturn);
            }
            catch (Exception ex)
            {
                logger.LogError(string.Format("Error in BLL - GetDataShareRequests. {0}", ex.Message), ex);
                String innerMessage = (ex.InnerException != null) ? ex.InnerException.Message : "";
                return GenericResponseBuilder.NoSuccess<ShapedDataWithLinks>(null, $"{ex.Message} InnerException: {innerMessage}");
            }
        }

        public async Task<GenericResponse> ReplyToDataShareRequest(Guid userId, DataShareRequestReplyDto dataShareRequestDto)
        {
            try
            {
                if (!(dataShareRequestDto.Reply.Equals(RequestStatusEnum.Accepted.ToString(), StringComparison.OrdinalIgnoreCase)
                    || dataShareRequestDto.Reply.Equals(RequestStatusEnum.Declined.ToString(), StringComparison.OrdinalIgnoreCase)))
                {
                    return GenericResponseBuilder.NoSuccess<bool>(false, "The method is for accepting or declining requests.");
                }
                var requesteeUserProfileExists = await GetUserProfileByUserId(userId,true);
                if (requesteeUserProfileExists.Result == null)
                {
                    return GenericResponseBuilder.NoSuccess<bool>(false, "User do not have a profile in the system.");
                }

                var requestExist = await this.dataService.DataShareRequests.FindByCondition(
                       d => (d.RequesterId == dataShareRequestDto.RequesterId)
                       && (d.RequesteeId == requesteeUserProfileExists.Result.Id)
                       && (d.RequestStatus.Description.Equals(RequestStatusEnum.Pending.ToString())), true);
                if (requestExist == null)
                {
                    return GenericResponseBuilder.NoSuccess<bool>(false, "Request do not exist in the system.");
                }

                var statusFromDb = await this
                    .dataService
                    .DataSharingRequestStatuses
                    .FindByCondition(d => d.Description.Equals(dataShareRequestDto.Reply.ToString()));

                if (dataShareRequestDto.Reply.Equals(RequestStatusEnum.Declined.ToString(), StringComparison.OrdinalIgnoreCase))
                {
                    requestExist.RequestStatus = statusFromDb;
                    this.dataService.DataShareRequests.Update(requestExist);
                    await this.dataService.CompleteAsync();
                    return GenericResponseBuilder.Success();
                }
                
                var farmsThatBelongToUser = requestExist
                    .Requestee.UserFarms
                    .Where(f => 
                        dataShareRequestDto.Farms.Any(d => f.FarmId.Equals(d))).ToList();

                if (farmsThatBelongToUser.Count == 0)
                {
                    return GenericResponseBuilder.NoSuccess<bool>(false, "Farms do not belong to the user.");
                }

                var requesterUserProfileExists = await this.dataService.UserProfiles.FindByIdAsync(dataShareRequestDto.RequesterId);
                if (requesterUserProfileExists == null)
                {
                    return GenericResponseBuilder.NoSuccess<bool>(false, "User do not have a profile in the system.");
                }

                foreach (var farm in farmsThatBelongToUser)
                {
                    await this.dataService.UserProfiles.AddFarm(
                        requesterUserProfileExists,
                        farm.Farm,
                        UserFarmTypeEnum.Advisor,
                        true);
                }
                requestExist.RequestStatus = statusFromDb;
                await this.dataService.CompleteAsync();

                return GenericResponseBuilder.Success();
            }
            catch (Exception ex)
            {
                logger.LogError(string.Format("Error in BLL - ReplyToDataShareRequest. {0}", ex.Message), ex);
                String innerMessage = (ex.InnerException != null) ? ex.InnerException.Message : "";
                return GenericResponseBuilder.NoSuccess($"{ex.Message} InnerException: {innerMessage}");
            }
        }

        #region Helpers

        #endregion
    }
}