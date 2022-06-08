using System;
using System.Collections.Generic;
using System.Linq;
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

        public async Task<bool> UserHasAnyDss(Guid userId)
        {
            try
            {
                return await this.dataService
                    .FieldCropPestDsses
                    .HasAny(f =>
                        f.FieldCropPest
                            .FieldCrop
                            .Field
                            .Farm
                            .UserFarms
                            .Any(fa => fa.UserId == userId));
            }
            catch (Exception ex)
            {
                logger.LogError(string.Format("Error in BLL - UserHasAnyDss. {0}", ex.Message), ex);
                return false;
            }
        }

        public async Task<List<ReportDataDto>> GetDataForReport()
        {
            try
            {
                var userFarms = await this.dataService
                    .UserFarms
                    .GetReportDataAsync();

                return this.mapper.Map<List<ReportDataDto>>(userFarms);
            }
            catch (Exception ex)
            {
                logger.LogError(string.Format("Error in BLL - GetDataForReport. {0}", ex.Message), ex);
                return null;
            }
        }
        #region Helpers

        #endregion
    }
}