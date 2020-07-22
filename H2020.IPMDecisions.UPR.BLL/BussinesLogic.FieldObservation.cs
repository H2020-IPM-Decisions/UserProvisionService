using System;
using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using H2020.IPMDecisions.UPR.Core.Dtos;
using H2020.IPMDecisions.UPR.Core.Entities;
using H2020.IPMDecisions.UPR.Core.Helpers;
using H2020.IPMDecisions.UPR.Core.Models;
using H2020.IPMDecisions.UPR.Core.ResourceParameters;
using Microsoft.AspNetCore.Http;

namespace H2020.IPMDecisions.UPR.BLL
{
    public partial class BusinessLogic : IBusinessLogic
    {
        public async  Task<GenericResponse<FieldObservationDto>> AddNewFieldObservation(FieldObservationForCreationDto fieldObservationForCreationDto, HttpContext httpContext, string mediaType)
        {
            try
            {
                var field = httpContext.Items["field"] as Field;

                var observationAsEntity = this.mapper.Map<FieldObservation>(fieldObservationForCreationDto);
                observationAsEntity.Field = field;

                // this.dataService.Fields.Create(observationAsEntity);
                await this.dataService.CompleteAsync();

                var fieldToReturn = this.mapper.Map<FieldObservationDto>(observationAsEntity);
                return GenericResponseBuilder.Success<FieldObservationDto>(fieldToReturn);
            }
            catch (Exception ex)
            {
                //ToDo Log Error
                return GenericResponseBuilder.NoSuccess<FieldObservationDto>(null, $"{ex.Message} InnerException: {ex.InnerException.Message}");
            }
        }

        public async Task<GenericResponse> DeleteFieldObservation(Guid id)
        {
            try
            {   
                return GenericResponseBuilder.Success();
            }
            catch (Exception ex)
            {
                //ToDo Log Error
                return GenericResponseBuilder.NoSuccess($"{ex.Message} InnerException: {ex.InnerException.Message}");
            }
        }

        public async Task<GenericResponse<FieldObservationDto>> GetFieldObservationDto(Guid id, string fields, string mediaType)
        {
            try
            {
                return GenericResponseBuilder.Success<FieldObservationDto>(null);
            }
            catch (Exception ex)
            {
                //ToDo Log Error
                return GenericResponseBuilder.NoSuccess<FieldObservationDto>(null, $"{ex.Message} InnerException: {ex.InnerException.Message}");
            }
        }

        public async Task<GenericResponse<ShapedDataWithLinks>> GetFieldObservations(Guid fieldId, object resourceParameter, string mediaType)
        {
            try
            {
                return GenericResponseBuilder.Success<ShapedDataWithLinks>(null);
            }
            catch (Exception ex)
            {
                //ToDo Log Error
                return GenericResponseBuilder.NoSuccess<ShapedDataWithLinks>(null, $"{ex.Message} InnerException: {ex.InnerException.Message}");
            }
        }
    }
}