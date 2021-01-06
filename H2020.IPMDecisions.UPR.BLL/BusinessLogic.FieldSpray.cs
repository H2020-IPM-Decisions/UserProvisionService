using System;
using System.Linq;
using System.Threading.Tasks;
using H2020.IPMDecisions.UPR.Core.Dtos;
using H2020.IPMDecisions.UPR.Core.Entities;
using H2020.IPMDecisions.UPR.Core.Models;
using H2020.IPMDecisions.UPR.Core.ResourceParameters;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace H2020.IPMDecisions.UPR.BLL
{
    public partial class BusinessLogic : IBusinessLogic
    {
        public async Task<GenericResponse<FieldSprayApplicationDto>> AddNewFieldSpray(FieldSprayApplicationForCreationDto sprayForCreationDto, HttpContext httpContext, string mediaType)
        {
            try
            {
                var field = httpContext.Items["field"] as Field;
                var fieldCropPestExist = field
                    .FieldCropPests
                    .Where(fcp => fcp.Id == sprayForCreationDto.FieldCropPestId)
                    .FirstOrDefault();

                if (fieldCropPestExist == null) return GenericResponseBuilder.NotFound<FieldSprayApplicationDto>();

                var objectAsEntity = this.mapper.Map<FieldSprayApplication>(sprayForCreationDto);
                objectAsEntity.FieldCropPest = fieldCropPestExist;

                this.dataService.FieldSprayApplication.Create(objectAsEntity);
                await this.dataService.CompleteAsync();

                var fieldToReturn = this.mapper.Map<FieldSprayApplicationDto>(objectAsEntity);
                return GenericResponseBuilder.Success(fieldToReturn);
            }
            catch (Exception ex)
            {
                logger.LogError(string.Format("Error in BLL - AddNewFieldSpray. {0}", ex.Message), ex);
                String innerMessage = (ex.InnerException != null) ? ex.InnerException.Message : "";
                return GenericResponseBuilder.NoSuccess<FieldSprayApplicationDto>(null, $"{ex.Message} InnerException: {innerMessage}");
            }
        }

        public Task<GenericResponse> DeleteFieldSpray(Guid id, HttpContext httpContext)
        {
            throw new NotImplementedException();
        }

        public GenericResponse<FieldSprayApplicationDto> GetFieldSprayDto(Guid id, string fields, string mediaType, HttpContext httpContext)
        {
            throw new NotImplementedException();
        }

        public Task<GenericResponse<ShapedDataWithLinks>> GetFieldSprays(Guid fieldId, FieldSprayResourceParameter resourceParameter, string mediaType)
        {
            throw new NotImplementedException();
        }
    }
}