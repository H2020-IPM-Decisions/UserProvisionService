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
    /// Crop Decisions are DSS applied to a crop and a pest.
    /// <para>Crop Decisions needs to be associate to a FieldCropPest. The FieldId must be associated to the UserId of the Authorization JWT.</para>
    /// <para>The user will be identified using the UserId on the authentication JWT.</para>
    /// </summary>
    [ApiController]
    [Route("api/fields/{fieldId:guid}/cropdecisions")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [ServiceFilter(typeof(AddUserIdToContextFilter), Order = 1)]
    [ServiceFilter(typeof(FieldBelongsToUserActionFilter), Order = 2)]
    public class FieldCropDecisionsController : ControllerBase
    {
        private readonly IBusinessLogic businessLogic;
        public FieldCropDecisionsController(IBusinessLogic businessLogic)
        {
            this.businessLogic = businessLogic
                ?? throw new System.ArgumentNullException(nameof(businessLogic));
        }

        /// <summary>Use this endpoint to remove a DSS that is associated with an field using its id.</summary>
        /// <remarks>The FieldCropPestId should be associated to the Field</remarks>
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpDelete("{id:guid}", Name = "api.fieldcropdecisions.delete.cropdecisionbyid")]
        //DELETE: api/fields/1/cropdecisions/1
        public async Task<IActionResult> Delete(
            [FromRoute] Guid fieldId, Guid id)
        {
            var response = await this.businessLogic.DeleteFieldCropDecision(id, HttpContext);
            if (!response.IsSuccessful)
                return BadRequest(new { message = response.ErrorMessage });

            return NoContent();
        }

        /// <summary>Use this endpoint to get the crop decisions that are associated with a field crop pest.
        /// </summary>
        /// <remarks>The FieldCropPestId must be associated to the Field</remarks>
        [ProducesResponseType(typeof(IEnumerable<FieldDssResultDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Produces(MediaTypeNames.Application.Json)]
        [HttpGet("", Name = "api.fieldcropdecisions.get.all")]
        [HttpHead]
        // GET: api/fields/1/cropdecisions
        public async Task<IActionResult> Get(
            [FromRoute] Guid fieldId,
            [FromQuery] FieldCropPestDssResourceParameter resourceParameter,
            [FromHeader(Name = "Accept")] string mediaType)
        {
            var response = await this.businessLogic.GetFieldCropDecisions(resourceParameter, HttpContext, mediaType);
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

        /// <summary>Use this endpoint to get a crop decisions by id.</summary>
        /// <remarks>The Id must be part of a FieldCropPest associated to the field</remarks>
        [ProducesResponseType(typeof(FieldDssResultDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Produces(MediaTypeNames.Application.Json)]
        [HttpGet("{id:guid}", Name = "api.fieldcropdecisions.get.cropdecisionbyid")]
        // GET: api/fields/1/cropdecisions/1
        public async Task<IActionResult> GetById(
            [FromRoute] Guid fieldId, Guid id,
            [FromHeader(Name = "Accept")] string mediaType)
        {
            var response = await this.businessLogic.GetFieldCropDecision(id, HttpContext, mediaType);
            if (!response.IsSuccessful)
                return response.RequestResult;

            return Ok(response.Result);
        }

        /// <summary>Use this end point to add a new crop decisions to an specific fieldCropPest of a field.</summary>
        /// <remarks>The FieldCropPestId must be associated to the Field
        /// <para>A 409 response will be returned if the FieldCropPest already have the DSS submitted.</para>
        /// </remarks>
        [Consumes(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [Produces(MediaTypeNames.Application.Json)]
        [HttpPost("", Name = "api.fieldcropdecisions.post.cropdecision")]
        // POST: api/fields/1/cropdecisions
        public async Task<IActionResult> Post(
            [FromRoute] Guid fieldId,
            [FromBody] CropPestDssForCreationDto cropPestDssForCreationDto,
            [FromHeader(Name = "Accept")] string mediaType)
        {
            var response = await this.businessLogic.AddNewFieldCropDecision(cropPestDssForCreationDto, HttpContext, mediaType);
            if (!response.IsSuccessful)
                return response.RequestResult;

            var route = "api.fieldcropdecisions.get.cropdecisionbyid";
            var routeValues = new
            {
                fieldId,
                id = response.Result["Id"]
            };
            var result = response.Result;

            if (response.RequestResult != null)
                return AcceptedAtRoute(route, routeValues, result);

            return CreatedAtRoute(route, routeValues, result);
        }

        /// <summary>Requests permitted on this URL</summary>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [HttpOptions]
        //OPTIONS: api/fields/1/cropdecisions
        public IActionResult Options([FromRoute] Guid fieldId)
        {
            Response.Headers.Add("Allow", "OPTIONS, GET, POST, DELETE");
            return Ok();
        }
    }
}