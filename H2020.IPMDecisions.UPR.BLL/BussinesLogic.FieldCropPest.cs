using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using H2020.IPMDecisions.UPR.Core.Dtos;
using H2020.IPMDecisions.UPR.Core.Models;
using H2020.IPMDecisions.UPR.Core.ResourceParameters;
using Microsoft.AspNetCore.Http;

namespace H2020.IPMDecisions.UPR.BLL
{
    public partial class BusinessLogic : IBusinessLogic
    {
        public Task<GenericResponse<CropPestDto>> AddNewFieldCropPest(CropPestForCreationDto cropPestForCreationDto, HttpContext httpContext, string mediaType)
        {
            throw new NotImplementedException();
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