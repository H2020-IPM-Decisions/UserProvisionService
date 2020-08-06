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
                var farmerUserId = await this.internalCommunicationProvider.GetUserIdFromIdpMicroservice(dataShareRequestDto.Email.ToString());
                if (string.IsNullOrEmpty(farmerUserId))
                {
                    return GenericResponseBuilder.NoSuccess<bool>(false, "User requested not in the system.");
                }

                var farmerProfile = await GetUserProfileByUserId(Guid.Parse(farmerUserId));
                if (farmerProfile.Result == null)
                {
                    return GenericResponseBuilder.NoSuccess<bool>(false, "User requested do not have a profile in the system.");
                }

                if (Guid.Parse(farmerUserId) == userId)
                {
                    return GenericResponseBuilder.NoSuccess<bool>(false, "User can not request its own data.");
                }

                var advisorProfile = await GetUserProfileByUserId(userId);
                if (advisorProfile.Result == null)
                {
                    return GenericResponseBuilder.NoSuccess<bool>(false, "User requesting data share do not have a profile in the system.");
                }

                var requestExists = await this.dataService.DataShareRequests.FindByCondition(
                        d => (d.RequesterId == userId)
                        && (d.RequesteeId == Guid.Parse(farmerUserId)), true);

                if (requestExists != null
                    && requestExists.RequestStatus.Description.Equals(RequestStatusEnum.Declined.ToString()))
                {
                    return GenericResponseBuilder.NoSuccess<bool>(false, "The user has declined access to the data.");
                }
                else if (requestExists != null)
                {
                    return GenericResponseBuilder.NoSuccess<bool>(false, "You have already requested data to this user.");
                }

                await this.dataService.DataShareRequests.Create(
                    userId,
                    Guid.Parse(farmerUserId),
                    RequestStatusEnum.Pending);
                await this.dataService.CompleteAsync();

                var requesterFullName = string.Format(
                    "{0} {1}",
                    advisorProfile.Result.FirstName,
                    advisorProfile.Result.LastName)
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
                    .FindAllAsync(userId, resourceParameter);

                if (requestsAsEntities.Count() == 0) return GenericResponseBuilder.NotFound<ShapedDataWithLinks>();

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
                var farmerProfile = await GetUserProfileByUserId(userId, true);
                if (farmerProfile.Result == null)
                {
                    return GenericResponseBuilder.NoSuccess<bool>(false, "User do not have a profile in the system.");
                }
                if (farmerProfile.Result.UserFarms == null || farmerProfile.Result.UserFarms.Count() == 0)
                {
                    return GenericResponseBuilder.NoSuccess<bool>(false, "User do not have any farms in the system.");
                }

                var requestExists = await this.dataService.DataShareRequests.FindByCondition(
                       d => (d.RequesterId == dataShareRequestDto.RequesterId)
                       && (d.RequesteeId == userId)
                       && (d.RequestStatus.Description.Equals(RequestStatusEnum.Pending.ToString())), true);
                if (requestExists == null)
                {
                    return GenericResponseBuilder.NoSuccess<bool>(false, "Request do not exist in the system or already accepted/declined.");
                }

                var statusFromDb = await this
                    .dataService
                    .DataSharingRequestStatuses
                    .FindByCondition(d => d.Description.Equals(dataShareRequestDto.Reply.ToString()));

                if (dataShareRequestDto.Reply.Equals(RequestStatusEnum.Declined.ToString(), StringComparison.OrdinalIgnoreCase))
                {
                    requestExists.RequestStatus = statusFromDb;
                    this.dataService.DataShareRequests.Update(requestExists);
                    await this.dataService.CompleteAsync();
                    return GenericResponseBuilder.Success();
                }

                var farmsThatBelongToUser = requestExists
                    .Requestee.UserFarms
                    .Where(f =>
                        dataShareRequestDto.Farms.Any(d => f.FarmId.Equals(d))).ToList();
                if (farmsThatBelongToUser.Count() == 0)
                {
                    return GenericResponseBuilder.NoSuccess<bool>(false, "Farms do not belong to the user.");
                }

                var advisorProfile = await this.dataService.UserProfiles.FindByIdAsync(dataShareRequestDto.RequesterId);
                if (advisorProfile == null)
                {
                    return GenericResponseBuilder.NoSuccess<bool>(false, "User do not have a profile in the system.");
                }

                foreach (var farm in farmsThatBelongToUser)
                {
                    await this.dataService.UserProfiles.AddFarm(
                        advisorProfile,
                        farm.Farm,
                        UserFarmTypeEnum.Advisor,
                        true);
                }
                requestExists.RequestStatus = statusFromDb;
                this.dataService.DataShareRequests.Update(requestExists);
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

        public async Task<GenericResponse> UpdateDataShareRequest(Guid userId, DataShareRequestUpdateDto dataShareRequestDto)
        {
            try
            {
                if ((dataShareRequestDto.Reply.Equals(
                    RequestStatusEnum.Pending.ToString(),
                    StringComparison.OrdinalIgnoreCase)))
                {
                    return GenericResponseBuilder.NoSuccess("You can not change a request back to pending.");
                }

                var farmerProfile = await GetUserProfileByUserId(userId, true);
                if (farmerProfile.Result == null)
                {
                    return GenericResponseBuilder.NoSuccess<bool>(false, "User do not have a profile in the system.");
                }
                if (farmerProfile.Result.UserFarms == null || farmerProfile.Result.UserFarms.Count() == 0)
                {
                    return GenericResponseBuilder.NoSuccess<bool>(false, "User do not have any farms in the system.");
                }

                var requestExists = await this.dataService.DataShareRequests.FindByCondition(
                       d => (d.RequesterId == dataShareRequestDto.RequesterId)
                       && (d.RequesteeId == userId), true);

                if (requestExists == null)
                {
                    return GenericResponseBuilder.NoSuccess<bool>(false, "Request do not exist in the system.");
                }
                else if (requestExists.RequestStatus.Description.Equals(
                    RequestStatusEnum.Pending.ToString(),
                    StringComparison.OrdinalIgnoreCase))
                {
                    return GenericResponseBuilder.NoSuccess("You can not update a pending request. Please use the 'reply' end point to accept/decline request.");
                }

                var statusFromDb = await this
                    .dataService
                    .DataSharingRequestStatuses
                    .FindByCondition(d => d.Description.Equals(dataShareRequestDto.Reply.ToString()));

                if (dataShareRequestDto.Reply.Equals(
                    RequestStatusEnum.Declined.ToString(),
                    StringComparison.OrdinalIgnoreCase))
                {
                    var advisorUserFarmsToDelete = new List<UserFarm>();
                    foreach (var farm in farmerProfile.Result.UserFarms)
                    {
                        var farmWithUserFarms = await this.dataService.Farms.FindByIdAsync(farm.FarmId);

                        if (farmWithUserFarms.UserFarms.Count() <= 1)
                            continue;

                        var advisorUserFarm = farmWithUserFarms.UserFarms
                                                    .Where(a => a.UserId == dataShareRequestDto.RequesterId)
                                                    .FirstOrDefault();

                        if (advisorUserFarm != null) 
                            advisorUserFarmsToDelete.Add(advisorUserFarm);
                    }

                    this.dataService.UserFarms.DeleteRange(advisorUserFarmsToDelete);                    
                }
                else
                {
                    var advisorProfile = await this.dataService.UserProfiles.FindByIdAsync(dataShareRequestDto.RequesterId);
                    if (advisorProfile == null)
                    {
                        return GenericResponseBuilder.NoSuccess<bool>(false, "User do not have a profile in the system.");
                    }

                    var farmsThatBelongToUser = requestExists
                        .Requestee
                        .UserFarms
                        .Where(f =>
                            dataShareRequestDto.Farms.Any(d => f.FarmId.Equals(d.FarmId))).ToList();

                    if (farmsThatBelongToUser.Count() == 0)
                    {
                        return GenericResponseBuilder.NoSuccess<bool>(false, "Farms do not belong to the user.");
                    }

                    foreach (var farm in farmsThatBelongToUser)
                    {
                        var farmWithUserFarms = await this.dataService.Farms.FindByIdAsync(farm.FarmId);
                        var advisorUserFarm = farmWithUserFarms.UserFarms
                                                    .Where(a => a.UserId == dataShareRequestDto.RequesterId)
                                                    .FirstOrDefault();
                        
                        var authorize = dataShareRequestDto
                                            .Farms
                                            .Where(f => f.FarmId == farm.FarmId)
                                            .Select(f => f.Authorize)
                                            .FirstOrDefault();

                        if (advisorUserFarm == null && authorize)
                        {
                            await this.dataService.UserProfiles.AddFarm(
                                    advisorProfile,
                                    farm.Farm,
                                    UserFarmTypeEnum.Advisor,
                                    true);
                        }
                        else if (advisorUserFarm != null && !authorize)
                        {
                            this.dataService.UserFarms.Delete(advisorUserFarm);
                        }
                    }
                }

                requestExists.RequestStatus = statusFromDb;
                this.dataService.DataShareRequests.Update(requestExists);
                await this.dataService.CompleteAsync();
                return GenericResponseBuilder.Success();
            }
            catch (Exception ex)
            {
                logger.LogError(string.Format("Error in BLL - UpdateDataShareRequest. {0}", ex.Message), ex);
                String innerMessage = (ex.InnerException != null) ? ex.InnerException.Message : "";
                return GenericResponseBuilder.NoSuccess($"{ex.Message} InnerException: {innerMessage}");
            }
        }

        #region Helpers

        #endregion
    }
}