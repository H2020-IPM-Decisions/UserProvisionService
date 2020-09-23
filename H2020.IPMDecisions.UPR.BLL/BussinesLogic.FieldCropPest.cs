using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using H2020.IPMDecisions.UPR.Core.Dtos;
using H2020.IPMDecisions.UPR.Core.Entities;
using H2020.IPMDecisions.UPR.Core.Helpers;
using H2020.IPMDecisions.UPR.Core.Models;
using H2020.IPMDecisions.UPR.Core.ResourceParameters;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace H2020.IPMDecisions.UPR.BLL
{
    public partial class BusinessLogic : IBusinessLogic
    {
        public async Task<GenericResponse<IDictionary<string, object>>> AddNewFieldCropPest(CropPestForCreationDto cropPestForCreationDto, HttpContext httpContext, string mediaType)
        {
            try
            {
                var field = httpContext.Items["field"] as Field;

                var cropPestExist = await this.dataService.CropPests
                        .FindByConditionAsync
                        (c => c.CropEppoCode == cropPestForCreationDto.CropEppoCode
                        && c.PestEppoCode == cropPestForCreationDto.PestEppoCode);

                if (cropPestExist == null)
                {
                    cropPestExist = this.mapper.Map<CropPest>(cropPestForCreationDto);
                    this.dataService.CropPests.Create(cropPestExist);

                    var newFieldCropPest = new FieldCropPest()
                    {
                        CropPest = cropPestExist,
                        Field = field
                    };

                    this.dataService.FieldCropPests.Create(newFieldCropPest);
                    await this.dataService.CompleteAsync();
                }
                else 
                {
                    var fieldCropPestExist = await this.dataService
                        .FieldCropPests
                        .FindByConditionAsync(f =>
                            f.FieldId == field.Id
                            & f.CropPestId == cropPestExist.Id);

                    if (fieldCropPestExist == null)
                    {
                        var newFieldCropPest = new FieldCropPest()
                        {
                            CropPest = cropPestExist,
                            Field = field
                        };

                        this.dataService.FieldCropPests.Create(newFieldCropPest);
                        await this.dataService.CompleteAsync();
                    }
                }                

                var cropPestToReturn = this.mapper
                    .Map<CropPestDto>(cropPestExist)
                    .ShapeData() as IDictionary<string, object>;
                return GenericResponseBuilder.Success<IDictionary<string, object>>(cropPestToReturn);
            }
            catch (Exception ex)
            {
                logger.LogError(string.Format("Error in BLL - AddNewFieldCropPest. {0}", ex.Message), ex);
                String innerMessage = (ex.InnerException != null) ? ex.InnerException.Message : "";
                return GenericResponseBuilder.NoSuccess<IDictionary<string, object>>(null, $"{ex.Message} InnerException: {innerMessage}");
            }
        }

        public Task<GenericResponse> DeleteFieldCropPest(Guid id)
        {
            throw new NotImplementedException();
        }

        public Task<GenericResponse<IDictionary<string, object>>> GetFieldCropPest(Guid id, string mediaType)
        {
            throw new NotImplementedException();
        }

        public Task<GenericResponse<ShapedDataWithLinks>> GetFieldCropPests(Guid fieldId, FieldCropPestResourceParameter resourceParameter, string mediaType)
        {
            throw new NotImplementedException();
        }
       
        #region Helpers
        
        #endregion
    }
}