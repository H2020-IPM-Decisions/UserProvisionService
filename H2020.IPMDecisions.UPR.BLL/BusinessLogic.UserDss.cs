using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using H2020.IPMDecisions.UPR.Core.Dtos;
using H2020.IPMDecisions.UPR.Core.Models;
using Microsoft.Extensions.Logging;

namespace H2020.IPMDecisions.UPR.BLL
{
    public partial class BusinessLogic : IBusinessLogic
    {
        public async Task<GenericResponse<List<FieldCropPestDssDto>>> GetAllUserFieldCropPestDss(Guid userId)
        {
            try
            {
                var fieldCropPestDssesAsEntities = await this.dataService.FieldCropPestDsses.FindAllAsync(f => f.FieldCropPest.FieldCrop.Field.Farm.UserFarms.Any(uf => uf.UserId == userId));
                var dataToReturn = this.mapper.Map<List<FieldCropPestDssDto>>(fieldCropPestDssesAsEntities);
                return GenericResponseBuilder.Success<List<FieldCropPestDssDto>>(dataToReturn);
            }
            catch (Exception ex)
            {
                logger.LogError(string.Format("Error in BLL - GetAllUserFieldCropPestDss. {0}", ex.Message), ex);
                String innerMessage = (ex.InnerException != null) ? ex.InnerException.Message : "";
                return GenericResponseBuilder.NoSuccess<List<FieldCropPestDssDto>>(null, $"{ex.Message} InnerException: {innerMessage}");
            }
        }

        public async Task<GenericResponse<FieldCropPestDssDto>> GetFieldCropPestDssById(Guid id, Guid userId)
        {
            try
            {
                var dss = await this.dataService.FieldCropPestDsses.FindByIdAsync(id);
                if (dss == null) return GenericResponseBuilder.NotFound<FieldCropPestDssDto>();

                var dssUserId = dss.FieldCropPest.FieldCrop.Field.Farm.UserFarms.FirstOrDefault().UserId;
                if (userId != dssUserId) return GenericResponseBuilder.NotFound<FieldCropPestDssDto>();

                var dataToReturn = this.mapper.Map<FieldCropPestDssDto>(dss);
                return GenericResponseBuilder.Success<FieldCropPestDssDto>(dataToReturn);
            }
            catch (Exception ex)
            {
                logger.LogError(string.Format("Error in BLL - GetFieldCropPestDssById. {0}", ex.Message), ex);
                String innerMessage = (ex.InnerException != null) ? ex.InnerException.Message : "";
                return GenericResponseBuilder.NoSuccess<FieldCropPestDssDto>(null, $"{ex.Message} InnerException: {innerMessage}");
            }
            throw new NotImplementedException();
        }
    }
}