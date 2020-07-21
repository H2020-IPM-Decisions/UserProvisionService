using System;
using System.Threading.Tasks;
using H2020.IPMDecisions.UPR.Core.Dtos;
using H2020.IPMDecisions.UPR.Core.Entities;
using H2020.IPMDecisions.UPR.Core.Models;
using Microsoft.AspNetCore.Http;

namespace H2020.IPMDecisions.UPR.BLL
{
    public partial class BusinessLogic : IBusinessLogic
    {
        public async Task<GenericResponse<FieldDto>> LinkNewFieldToFarm(
            FieldForCreationDto fieldForCreationDto,
            HttpContext httpContext, 
            string mediaType)
        {
            try
            {
                var userId = Guid.Parse(httpContext.Items["userId"].ToString());
                var farm = httpContext.Items["farm"] as Farm;

                var fieldAsEntity = this.mapper.Map<Field>(fieldForCreationDto);
                fieldAsEntity.Farm = farm;

                this.dataService.Fields.Create(fieldAsEntity);
                await this.dataService.CompleteAsync();

                var fieldToReturn = this.mapper.Map<FieldDto>(fieldAsEntity);
                return GenericResponseBuilder.Success<FieldDto>(fieldToReturn);
            }
            catch (Exception ex)
            {
                //ToDo Log Error
                return GenericResponseBuilder.NoSuccess<FieldDto>(null, $"{ex.Message} InnerException: {ex.InnerException.Message}");
            }
        }
    }
}