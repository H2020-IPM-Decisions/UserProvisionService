using System;
using System.Linq;
using System.Threading.Tasks;
using H2020.IPMDecisions.UPR.Core.Dtos;
using H2020.IPMDecisions.UPR.Core.Models;
using Microsoft.Extensions.Logging;

namespace H2020.IPMDecisions.UPR.BLL
{
    public partial class BusinessLogic : IBusinessLogic
    {
        public async Task<GenericResponse<AdaptationDashboardDto>> GetAdaptationDataById(Guid id, Guid userId)
        {
            try
            {
                var dss = await this.dataService.FieldCropPestDsses.FindByIdAsync(id);
                if (dss == null) return GenericResponseBuilder.NotFound<AdaptationDashboardDto>();

                var dssUserId = dss.FieldCropPest.FieldCrop.Field.Farm.UserFarms.FirstOrDefault().UserId;
                if (userId != dssUserId) return GenericResponseBuilder.NotFound<AdaptationDashboardDto>();

                var dataToReturn = new AdaptationDashboardDto()
                {
                    DssOriginalParameters = await GenerateUserDssParameters(dss),
                    DssOriginalResult = await CreateDetailedResultToReturn(dss)
                };

                return GenericResponseBuilder.Success<AdaptationDashboardDto>(dataToReturn);
            }
            catch (Exception ex)
            {
                logger.LogError(string.Format("Error in BLL - GetFieldCropPestDssParametersById. {0}", ex.Message), ex);
                String innerMessage = (ex.InnerException != null) ? ex.InnerException.Message : "";
                return GenericResponseBuilder.NoSuccess<AdaptationDashboardDto>(null, $"{ex.Message} InnerException: {innerMessage}");
            }
        }
    }
}