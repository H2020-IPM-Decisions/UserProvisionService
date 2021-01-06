using System;
using System.Threading.Tasks;
using H2020.IPMDecisions.UPR.Core.Dtos;
using H2020.IPMDecisions.UPR.Core.Models;
using H2020.IPMDecisions.UPR.Core.ResourceParameters;
using Microsoft.AspNetCore.Http;

namespace H2020.IPMDecisions.UPR.BLL
{
    public partial class BusinessLogic : IBusinessLogic
    {
        public Task<GenericResponse<FieldSprayApplicationDto>> AddNewFieldSpray(FieldSprayApplicationForCreationDto sprayForCreationDto, HttpContext httpContext, string mediaType)
        {
            throw new NotImplementedException();
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