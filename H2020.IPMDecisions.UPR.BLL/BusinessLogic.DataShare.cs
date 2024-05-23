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
        public async Task<GenericResponse> DeleteDataShareRequest(Guid id, Guid userId)
        {
            try
            {
                var requestsAsEntity = await this
                    .dataService
                    .DataShareRequests
                    .FindByConditionAsync(d => d.Id == id &
                        (d.RequesteeId == userId || d.RequesterId == userId), true);

                if (requestsAsEntity == null)
                {
                    return GenericResponseBuilder.NoSuccess(this.jsonStringLocalizer["data_share.request_error"].ToString());
                }

                var advisorUserFarmsToDelete = await GetListOfFarmsSharedAsync(requestsAsEntity.Requestee.UserFarms, requestsAsEntity.RequesterId);

                this.dataService.UserFarms.DeleteRange(advisorUserFarmsToDelete);
                this.dataService.DataShareRequests.Delete(requestsAsEntity);
                await this.dataService.CompleteAsync();

                return GenericResponseBuilder.Success();
            }
            catch (Exception ex)
            {
                logger.LogError(string.Format("Error in BLL - DeleteDataShareRequest. {0}", ex.Message));
                String innerMessage = (ex.InnerException != null) ? ex.InnerException.Message : "";
                return GenericResponseBuilder.NoSuccess<bool>(false, $"{ex.Message} InnerException: {innerMessage}");
            }
        }

        public async Task<GenericResponse<bool>> AddDataShareRequest(Guid userId, DataShareRequestForCreationDto dataShareRequestDto)
        {
            try
            {
                var farmerUserInformation = await this.internalCommunicationProvider.GetUserInformationFromIdpMicroservice(dataShareRequestDto.Email.ToString());
                if (farmerUserInformation == null || string.IsNullOrEmpty(farmerUserInformation.Id.ToString()))
                {
                    return GenericResponseBuilder.NoSuccess<bool>(false, this.jsonStringLocalizer["data_share.user_not_registered"].ToString());
                }

                var farmerProfile = await GetUserProfileByUserId(farmerUserInformation.Id);
                if (farmerProfile.Result == null)
                {
                    return GenericResponseBuilder.NoSuccess<bool>(false, this.jsonStringLocalizer["data_share.user_without_profile"].ToString());
                }

                if (farmerUserInformation.Id == userId)
                {
                    return GenericResponseBuilder.NoSuccess<bool>(false, this.jsonStringLocalizer["data_share.own_request_error"].ToString());
                }

                var advisorProfile = await GetUserProfileByUserId(userId);
                if (advisorProfile.Result == null)
                {
                    return GenericResponseBuilder.NoSuccess<bool>(false, this.jsonStringLocalizer["data_share.user_requesting_no_profile"].ToString());
                }

                var requestExists = await this.dataService.DataShareRequests.FindByConditionAsync(
                        d => (d.RequesterId == userId)
                        && (d.RequesteeId == farmerUserInformation.Id), true);

                if (requestExists != null
                    && requestExists.RequestStatus.Description.Equals(RequestStatusEnum.Declined.ToString()))
                {
                    return GenericResponseBuilder.NoSuccess<bool>(false, this.jsonStringLocalizer["data_share.declined_request"].ToString());
                }
                else if (requestExists != null)
                {
                    return GenericResponseBuilder.NoSuccess<bool>(false, this.jsonStringLocalizer["data_share.existing_request"].ToString());
                }

                await this.dataService.DataShareRequests.Create(
                    userId,
                    farmerUserInformation.Id,
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
                    return GenericResponseBuilder.NoSuccess<bool>(false, this.jsonStringLocalizer["data_share.email_error"].ToString());
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
                    return GenericResponseBuilder.NoSuccess<ShapedDataWithLinks>(null, this.jsonStringLocalizer["shared.wrong_order_by"].ToString());

                var userProfile = await GetUserProfileByUserId(userId);
                if (userProfile.Result == null)
                    return GenericResponseBuilder.NoSuccess<ShapedDataWithLinks>(null, this.jsonStringLocalizer["shared.missing_user_profile"].ToString());

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

                var farmsFromUser = await this.dataService.Farms.FindAllByConditionAsync(f => f.UserFarms.Any(uf => uf.UserId == userId & (uf.Authorised)));
                var requestsAsDto = this.mapper
                   .Map<IEnumerable<DataShareRequestDto>>(requestsAsEntities);

                foreach (var item in requestsAsDto)
                {
                    item.AuthorizedFarms = farmsFromUser
                        .Where(f => f.UserFarms.Count > 1 && f.UserFarms.Any(uf => uf.UserId.Equals(item.RequesterId)))
                        .Select(uf => uf.Id)
                        .ToList();
                };

                var shapedRequestsToReturn = requestsAsDto.ShapeData();
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
                    return GenericResponseBuilder.NoSuccess<bool>(false, this.jsonStringLocalizer["data_share.wrong_method"].ToString());
                }
                var farmerProfile = await GetUserProfileByUserId(userId, true);
                if (farmerProfile.Result == null)
                {
                    return GenericResponseBuilder.NoSuccess<bool>(false, this.jsonStringLocalizer["data_share.user_without_profile"].ToString());
                }
                if (farmerProfile.Result.UserFarms == null || farmerProfile.Result.UserFarms.Count() == 0)
                {
                    return GenericResponseBuilder.NoSuccess<bool>(false, this.jsonStringLocalizer["data_share.no_farms_error"].ToString());
                }

                var requestExists = await this.dataService.DataShareRequests.FindByConditionAsync(
                       d => (d.RequesterId == dataShareRequestDto.RequesterId)
                       && (d.RequesteeId == userId)
                       && (d.RequestStatus.Description.Equals(RequestStatusEnum.Pending.ToString())), true);
                if (requestExists == null)
                {
                    return GenericResponseBuilder.NoSuccess<bool>(false, this.jsonStringLocalizer["data_share.not_existing_request"].ToString());
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

                List<UserFarm> farmsThatBelongToUser = new List<UserFarm>();

                if (dataShareRequestDto.AllowAllFarms)
                {
                    farmsThatBelongToUser = requestExists
                        .Requestee
                        .UserFarms
                        .ToList();
                }
                else
                {
                    farmsThatBelongToUser = requestExists
                        .Requestee
                        .UserFarms
                        .Where(f =>
                            dataShareRequestDto.Farms.Any(d => f.FarmId.Equals(d))).ToList();
                }

                if (farmsThatBelongToUser.Count() == 0)
                {
                    return GenericResponseBuilder.NoSuccess<bool>(false, this.jsonStringLocalizer["data_share.farms_ownership_error"].ToString());
                }

                var advisorProfile = await this.dataService.UserProfiles.FindByIdAsync(dataShareRequestDto.RequesterId);
                if (advisorProfile == null)
                {
                    return GenericResponseBuilder.NoSuccess<bool>(false, this.jsonStringLocalizer["data_share.user_without_profile"].ToString());
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
                    return GenericResponseBuilder.NoSuccess(this.jsonStringLocalizer["data_share.back_to_pending"].ToString());
                }

                var farmerProfile = await GetUserProfileByUserId(userId, true);
                if (farmerProfile.Result == null)
                {
                    return GenericResponseBuilder.NoSuccess<bool>(false, this.jsonStringLocalizer["data_share.user_without_profile"].ToString());
                }
                if (farmerProfile.Result.UserFarms == null || farmerProfile.Result.UserFarms.Count() == 0)
                {
                    return GenericResponseBuilder.NoSuccess<bool>(false, this.jsonStringLocalizer["data_share.no_farms_error"].ToString());
                }

                var requestExists = await this.dataService.DataShareRequests.FindByConditionAsync(
                       d => (d.RequesterId == dataShareRequestDto.RequesterId)
                       && (d.RequesteeId == userId), true);

                if (requestExists == null)
                {
                    return GenericResponseBuilder.NoSuccess<bool>(false, this.jsonStringLocalizer["data_share.not_existing_request"].ToString());
                }
                else if (requestExists.RequestStatus.Description.Equals(
                    RequestStatusEnum.Pending.ToString(),
                    StringComparison.OrdinalIgnoreCase))
                {
                    return GenericResponseBuilder.NoSuccess(this.jsonStringLocalizer["data_share.pending_change_error"].ToString());
                }

                if (dataShareRequestDto.Reply.Equals(
                    RequestStatusEnum.Declined.ToString(),
                    StringComparison.OrdinalIgnoreCase))
                {
                    var advisorUserFarmsToDelete = await GetListOfFarmsSharedAsync(farmerProfile.Result.UserFarms, dataShareRequestDto.RequesterId);
                    this.dataService.UserFarms.DeleteRange(advisorUserFarmsToDelete);
                }
                else
                {
                    var advisorProfile = await this.dataService.UserProfiles.FindByIdAsync(dataShareRequestDto.RequesterId);
                    if (advisorProfile == null)
                    {
                        return GenericResponseBuilder.NoSuccess<bool>(false, this.jsonStringLocalizer["data_share.user_without_profile"].ToString());
                    }

                    var farmsThatBelongToUser = requestExists
                        .Requestee
                        .UserFarms
                        .Where(f =>
                            dataShareRequestDto.Farms.Any(d => f.FarmId.Equals(d.FarmId))).ToList();

                    if (farmsThatBelongToUser.Count() == 0)
                    {
                        return GenericResponseBuilder.NoSuccess<bool>(false, this.jsonStringLocalizer["data_share.farms_ownership_error"].ToString());
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

                var statusFromDb = await this
                    .dataService
                    .DataSharingRequestStatuses
                    .FindByCondition(d => d.Description.Equals(dataShareRequestDto.Reply.ToString()));

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
        private async Task<List<UserFarm>> GetListOfFarmsSharedAsync(IList<UserFarm> requesteeFarms, Guid requesterId)
        {
            var advisorUserFarms = new List<UserFarm>();
            foreach (var farm in requesteeFarms)
            {
                var farmWithUserFarms = await this.dataService.Farms.FindByIdAsync(farm.FarmId);
                if (farmWithUserFarms.UserFarms.Count() <= 1)
                    continue;

                var advisorUserFarm = farmWithUserFarms.UserFarms
                                            .Where(a => a.UserId == requesterId)
                                            .FirstOrDefault();
                if (advisorUserFarm != null)
                    advisorUserFarms.Add(advisorUserFarm);
            }
            return advisorUserFarms;
        }
        #endregion
    }
}