using System;
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
        public async Task<GenericResponse<FieldDssResultDto>> GetLatestFieldCropPestDssResult(Guid dssId, Guid userId)
        {
            try
            {
                var dss = await this.dataService.FieldCropPestDsses.FindByIdAsync(dssId);
                if (dss == null) return GenericResponseBuilder.NotFound<FieldDssResultDto>();

                var dssUserId = dss.FieldCropPest.FieldCrop.Field.Farm.UserFarms.FirstOrDefault().UserId;
                if (userId != dssUserId) return GenericResponseBuilder.NotFound<FieldDssResultDto>();

                var dssResultAsEntity = dss.FieldDssResults.OrderByDescending(f => f.CreationDate).FirstOrDefault();
                var dssResultToReturnEntity = this.mapper.Map<FieldDssResultDto>(dssResultAsEntity);
                return GenericResponseBuilder.Success<FieldDssResultDto>(dssResultToReturnEntity);
            }
            catch (Exception ex)
            {
                logger.LogError(string.Format("Error in BLL - CreateFieldCropPestDssResult. {0}", ex.Message));
                String innerMessage = (ex.InnerException != null) ? ex.InnerException.Message : "";
                return GenericResponseBuilder.NoSuccess<FieldDssResultDto>(null, $"{ex.Message} InnerException: {innerMessage}");
            }
        }

        public async Task<GenericResponse<FieldDssResultDto>> CreateFieldCropPestDssResult(Guid dssId, Guid userId, FieldDssResultForCreationDto dssResultDto)
        {
            try
            {
                var dss = await this.dataService.FieldCropPestDsses.FindByIdAsync(dssId);
                if (dss == null) return GenericResponseBuilder.NotFound<FieldDssResultDto>();

                var dssUserId = dss.FieldCropPest.FieldCrop.Field.Farm.UserFarms.FirstOrDefault().UserId;
                if (userId != dssUserId) return GenericResponseBuilder.NotFound<FieldDssResultDto>();

                var dssResultAsEntity = this.mapper.Map<FieldDssResult>(dssResultDto);
                this.dataService.FieldCropPestDsses.AddDssResult(dss, dssResultAsEntity);
                await this.dataService.CompleteAsync();

                var dssResultToReturnEntity = this.mapper.Map<FieldDssResultDto>(dssResultAsEntity);
                return GenericResponseBuilder.Success<FieldDssResultDto>(dssResultToReturnEntity);
            }
            catch (Exception ex)
            {
                logger.LogError(string.Format("Error in BLL - CreateFieldCropPestDssResult. {0}", ex.Message));
                String innerMessage = (ex.InnerException != null) ? ex.InnerException.Message : "";
                return GenericResponseBuilder.NoSuccess<FieldDssResultDto>(null, $"{ex.Message} InnerException: {innerMessage}");
            }
        }
    }
}