using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using H2020.IPMDecisions.UPR.Core.Dtos;
using H2020.IPMDecisions.UPR.Core.Entities;
using H2020.IPMDecisions.UPR.Core.Helpers;
using H2020.IPMDecisions.UPR.Core.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace H2020.IPMDecisions.UPR.BLL
{
    public partial class BusinessLogic : IBusinessLogic
    {
        public async Task<GenericResponse<IDictionary<string, object>>> AddNewFieldCropDecision(
            CropPestDssForCreationDto cropPestDssForCreationDto,
            HttpContext httpContext,
            string mediaType)
        {
            try
            {
                var field = httpContext.Items["field"] as Field;

                var duplicatedRecord = field
                    .FieldCropPests          
                    .Any(f => f.FieldCropPestDsses
                        .Any(fcpd =>
                            fcpd.FieldCropPestId == cropPestDssForCreationDto.FieldCropPestId
                            & fcpd.CropPestDss.DssId == cropPestDssForCreationDto.DssId));                
                if (duplicatedRecord)
                    return GenericResponseBuilder.Duplicated<IDictionary<string, object>>();                

                var getFieldCropPest = await this.dataService
                    .FieldCropPests
                        .FindByConditionAsync
                        (f => f.Id == cropPestDssForCreationDto.FieldCropPestId
                        & f.FieldId == field.Id);
                if (getFieldCropPest == null)
                    return GenericResponseBuilder.NotFound<IDictionary<string, object>>();

                var cropPestDssExist = await this.dataService
                    .CropPestDsses
                    .FindByConditionAsync(c =>
                        c.CropPestId == getFieldCropPest.CropPestId
                        & c.DssId== cropPestDssForCreationDto.DssId);

                if (cropPestDssExist == null)
                {
                    cropPestDssExist = new CropPestDss()
                    {
                        CropPestId = getFieldCropPest.CropPestId,
                        DssId = cropPestDssForCreationDto.DssId,
                        DssName = cropPestDssForCreationDto.DssId
                    };
                    this.dataService.CropPestDsses.Create(cropPestDssExist);
                }
                
                var newFieldCropPestDss = new FieldCropPestDss()
                {
                    FieldCropPestId = cropPestDssForCreationDto.FieldCropPestId,
                    CropPestDssId = cropPestDssExist.Id
                };
                this.dataService.FieldCropPestDsses.Create(newFieldCropPestDss);
                await this.dataService.CompleteAsync();

                var fieldCropPestToReturn = this.mapper
                    .Map<FieldCropPestDssDto>(newFieldCropPestDss)
                    .ShapeData() as IDictionary<string, object>;
                return GenericResponseBuilder.Success<IDictionary<string, object>>(fieldCropPestToReturn);
            }
            catch (Exception ex)
            {
                logger.LogError(string.Format("Error in BLL - AddNewFieldCroDecision. {0}", ex.Message), ex);
                String innerMessage = (ex.InnerException != null) ? ex.InnerException.Message : "";
                return GenericResponseBuilder.NoSuccess<IDictionary<string, object>>(null, $"{ex.Message} InnerException: {innerMessage}");
            }
        }
       
        #region Helpers
        
        #endregion
    }
}