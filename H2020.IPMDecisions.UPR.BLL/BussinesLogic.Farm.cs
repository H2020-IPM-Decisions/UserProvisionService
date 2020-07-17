using System;
using Microsoft.Net.Http.Headers;
using System.Threading.Tasks;
using H2020.IPMDecisions.UPR.Core.Dtos;
using H2020.IPMDecisions.UPR.Core.Entities;
using H2020.IPMDecisions.UPR.Core.Models;
using Microsoft.AspNetCore.Http;
using System.Linq;

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
                    return GenericResponseBuilder.NoSuccess<FarmDto>(null, "Please add User Profile first.");
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

        public async Task<GenericResponse<FarmDto>> GetFarmById(Guid id, string fields, HttpContext context, string mediaType)
        {
            try
            {
                if (!MediaTypeHeaderValue.TryParse(mediaType,
                       out MediaTypeHeaderValue parsedMediaType))
                    return GenericResponseBuilder.NoSuccess<FarmDto>(null, "Wrong media type.");

                var userId = Guid.Parse(context.Items["userId"].ToString());
                var isAdmin = context.Items["isAdmin"];

                Farm farmAsEntity = null;

                if (isAdmin == null)
                {
                    farmAsEntity = await this
                        .dataService
                        .Farms
                        .FindByCondition(
                            f => f.Id == id &&
                            f.UserFarms
                                .Any(uf => uf.UserId == userId));
                }
                else
                {
                    farmAsEntity = await this
                        .dataService
                        .Farms
                        .FindByIdAsync(id);
                }

                if (farmAsEntity == null) return GenericResponseBuilder.NotFound<FarmDto>();

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