using System;
using System.Collections.Generic;
using Microsoft.Net.Http.Headers;
using System.Threading.Tasks;
using H2020.IPMDecisions.UPR.Core.Dtos;
using H2020.IPMDecisions.UPR.Core.Entities;
using H2020.IPMDecisions.UPR.Core.Models;
using H2020.IPMDecisions.UPR.Core.Helpers;
using NetTopologySuite.Geometries;

namespace H2020.IPMDecisions.UPR.BLL
{
    public partial class BusinessLogic : IBusinessLogic
    {
        public async Task<GenericResponse<FarmDto>> LinkNewFarmToUserProfile(FarmForCreationDto farmForCreation, Guid userId, string mediaType)
        {
            try
            {
                if (!MediaTypeHeaderValue.TryParse(mediaType,
                       out MediaTypeHeaderValue parsedMediaType))
                    return GenericResponseBuilder.NoSuccess<FarmDto>(null, "Wrong media type.");

                var userProfile = await GetUserProfile(userId);
                if (userProfile.Result == null)
                {
                    // ToDo Create empty profile
                }
                var farmAsEntity = this.mapper.Map<Farm>(farmForCreation);

                this.dataService.UserProfiles.AddFarm(userProfile.Result, farmAsEntity);
                await this.dataService.CompleteAsync();

                var farmToReturn = this.mapper.Map<FarmDto>(farmAsEntity);

                return GenericResponseBuilder.Success<FarmDto>(farmToReturn);
            }
            catch (Exception ex)
            {
                //ToDo Log Error
                return GenericResponseBuilder.NoSuccess<FarmDto>(null, $"{ex.Message} InnerException: {ex.InnerException.Message}");
            }
        }
    }
}