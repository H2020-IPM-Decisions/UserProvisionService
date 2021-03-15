using System;
using System.Collections.Generic;
using System.Net.Mime;
using System.Text.Json;
using System.Threading.Tasks;
using H2020.IPMDecisions.UPR.API.Filters;
using H2020.IPMDecisions.UPR.BLL;
using H2020.IPMDecisions.UPR.Core.Dtos;
using H2020.IPMDecisions.UPR.Core.ResourceParameters;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace H2020.IPMDecisions.UPR.API.Controllers
{
    /// <summary>
    /// Field Observations are observations of a pest on a field.
    /// <para>Observations needs to be associate to a FieldCropPest. The FieldId must be associated to the UserId of the Authorization JWT.</para>
    /// <para>The user will be identified using the UserId on the authentification JWT.</para>
    /// </summary>
    [ApiController]
    [Route("api/fields/{fieldId:guid}/observations")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [ServiceFilter(typeof(AddUserIdToContextFilter), Order = 1)]
    [ServiceFilter(typeof(FieldBelongsToUserActionFilter), Order = 2)]
    public class FieldObservationsController : ControllerBase
    {
        private readonly IBusinessLogic businessLogic;
        public FieldObservationsController(IBusinessLogic businessLogic)
        {
            this.businessLogic = businessLogic
                ?? throw new System.ArgumentNullException(nameof(businessLogic));
        }

        /// <summary>Use this endpoint to remove an observation that is associated with an field using its id.</summary>
        /// <remarks>The FieldCropPestId should be associated to the Field</remarks>
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpDelete("{id:guid}", Name = "api.observation.delete.observationbyid")]
        //DELETE: api/fields/1/observations/1
        public async Task<IActionResult> Delete(
            [FromRoute] Guid fieldId, Guid id)
        {
            var response = await this.businessLogic.DeleteFieldObservation(id, HttpContext);

            if (!response.IsSuccessful)
                return BadRequest(new { message = response.ErrorMessage });

            return NoContent();
        }

        /// <summary>Use this endpoint to get the observations that are associated with a field crop pest.
        /// </summary>
        /// <remarks>The FieldCropPestId must be associated to the Field</remarks>
        [ProducesResponseType(typeof(IEnumerable<FieldObservationDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Produces(MediaTypeNames.Application.Json)]
        [HttpGet("", Name = "api.observation.get.all")]
        [HttpHead]
        // GET: api/fields/1/observations
        public async Task<IActionResult> Get(
            [FromRoute] Guid fieldId,
            [FromQuery] FieldObservationResourceParameter resourceParameter,
            [FromHeader(Name = "Accept")] string mediaType)
        {
            var response = await this.businessLogic.GetFieldObservations(fieldId, resourceParameter, mediaType);

            if (!response.IsSuccessful)
                return response.RequestResult;

            Response.Headers.Add("X-Pagination",
                JsonSerializer.Serialize(response.Result.PaginationMetaData));

            return Ok(new
            {
                value = response.Result.Value,
                links = response.Result.Links
            });
        }

        /// <summary>Use this endpoint to get the observations by id.</summary>
        /// <remarks>The Id must be part of a FieldCropPest associated to the field</remarks>
        [ProducesResponseType(typeof(FieldObservationDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Produces(MediaTypeNames.Application.Json)]
        [HttpGet("{id:guid}", Name = "api.observation.get.observationbyid")]
        // GET:  api/fields/1/observations/1
        public IActionResult GetFieldObservationById(
            [FromRoute] Guid fieldId, Guid id,
            [FromQuery] string fields,
            [FromHeader(Name = "Accept")] string mediaType)
        {
            var response = this.businessLogic.GetFieldObservationDto(id, fields, mediaType, HttpContext);

            if (!response.IsSuccessful)
                return response.RequestResult;

            return Ok(response.Result);
        }

        /// <summary>Use this end point to add a new observation to an specific fieldCropPest of a field.</summary>
        /// <remarks>The FieldCropPestId must be associated to the Field</remarks>
        [Consumes(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Produces(MediaTypeNames.Application.Json)]
        [HttpPost("", Name = "api.observation.post.observation")]
        // POST: api/fields/1/observations
        public async Task<IActionResult> Post(
            [FromRoute] Guid fieldId,
            [FromBody] FieldObservationForCreationDto observationForCreationDto,
            [FromHeader(Name = "Accept")] string mediaType)
        {
            var response = await this.businessLogic.AddNewFieldObservation(observationForCreationDto, HttpContext, mediaType);

            if (!response.IsSuccessful)
                return response.RequestResult;

            return CreatedAtRoute(
                "api.observation.get.observationbyid",
                new
                {
                    fieldId,
                    id = response.Result.Id
                },
                response.Result);
        }

        /// <summary>Requests permitted on this URL</summary>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [HttpOptions]
        //OPTIONS: api/fields/1/observations
        public IActionResult Options([FromRoute] Guid fieldId)
        {
            Response.Headers.Add("Allow", "OPTIONS, GET, POST, DELETE");
            return Ok();
        }
    }
}