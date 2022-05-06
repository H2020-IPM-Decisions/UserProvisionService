using System;
using System.Linq;
using System.Threading.Tasks;
using H2020.IPMDecisions.UPR.BLL.Helpers;
using H2020.IPMDecisions.UPR.Core.Dtos;
using H2020.IPMDecisions.UPR.Core.Models;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;

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

                var dssInputUISchema = await internalCommunicationProvider
                                       .GetDssModelInputSchemaMicroservice(dss.CropPestDss.DssId, dss.CropPestDss.DssModelId);
                JObject inputAsJsonObject = null;
                if (dssInputUISchema != null)
                {
                    inputAsJsonObject = JObject.Parse(dssInputUISchema.ToString());
                    JObject userParametersAsJsonObject = JObject.Parse(dss.DssParameters.ToString());
                    DssDataHelper.AddDefaultDssParametersToInputSchema(inputAsJsonObject, userParametersAsJsonObject);
                }
                var dataToReturn = new AdaptationDashboardDto()
                {
                    DssOriginalParameters = inputAsJsonObject

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