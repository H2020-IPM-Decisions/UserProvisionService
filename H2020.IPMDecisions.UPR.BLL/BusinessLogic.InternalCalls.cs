using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using H2020.IPMDecisions.UPR.Core.Dtos;
using H2020.IPMDecisions.UPR.Core.Entities;
using H2020.IPMDecisions.UPR.Core.Models;
using Microsoft.Extensions.Logging;
using Two10.CountryLookup;

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

        public async Task<List<ReportData>> GetDataForReport()
        {
            try
            {
                var userFarms = await this.dataService
                    .UserFarms
                    .GetReportDataAsync();

                // Creating a ReverseLookup object is expensive, so it's worth keeping it as a singleton.
                var lookup = new ReverseLookup();
                var dataToReturn = new List<ReportData>();
                foreach (var userFarm in userFarms)
                {
                    var newItem = this.mapper.Map<ReportData>(userFarm);
                    newItem.Farm.Country = lookup.Lookup((float)userFarm.Farm.Location.Coordinate.Y, (float)userFarm.Farm.Location.X).Name.ToString();
                    var listOfFieldCropPestDss = new List<CropPestDss>();
                    // How you do this in automapper!!??!! 
                    // Improve process with dictionaries to improve efficiency
                    if (userFarm.Farm.Fields == null) continue;
                    foreach (var field in userFarm.Farm.Fields)
                    {
                        if (field.FieldCrop.FieldCropPests == null) continue;
                        foreach (var fieldCropPests in field.FieldCrop.FieldCropPests)
                        {
                            if (fieldCropPests.CropPest.CropPestDsses == null) continue;
                            foreach (var cropPestDss in fieldCropPests.CropPest.CropPestDsses)
                            {
                                listOfFieldCropPestDss.Add(cropPestDss);
                            }
                        }
                    }
                    newItem.Farm.DssModels = this.mapper.Map<List<ReportDataDssModel>>(listOfFieldCropPestDss);
                    dataToReturn.Add(newItem);
                }
                return dataToReturn;
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