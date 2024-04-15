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
    /// Field Sprays are spray applications against a pest on a field.
    /// <para>Spray applications needs to be associate to a FieldCropPest. The FieldId must be associated to the UserId of the Authorization JWT.</para>
    /// <para>The user will be identified using the UserId on the authentication JWT.</para>
    /// </summary>
    [ApiController]
    [Route("api/fields/{fieldId:guid}/sprayapplications")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [ServiceFilter(typeof(AddUserIdToContextFilter), Order = 1)]
    [ServiceFilter(typeof(FieldBelongsToUserActionFilter), Order = 2)]
    public class FieldSprayApplicationsController : ControllerBase
    {
        private readonly IBusinessLogic businessLogic;
        public FieldSprayApplicationsController(IBusinessLogic businessLogic)
        {
            this.businessLogic = businessLogic
                ?? throw new System.ArgumentNullException(nameof(businessLogic));
        }

        /// <summary>Use this endpoint to remove an spray application that is associated with an field using its id.</summary>
        /// <remarks>The FieldCropPestId must be associated to the Field</remarks>
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpDelete("{id:guid}", Name = "api.spray.delete.spraybyid")]
        //DELETE: api/fields/1/sprayapplications/1
        public async Task<IActionResult> Delete(
            [FromRoute] Guid fieldId, Guid id)
        {
            var response = await this.businessLogic.DeleteFieldSpray(id, HttpContext);

            if (!response.IsSuccessful)
                return BadRequest(new { message = response.ErrorMessage });

            return NoContent();
        }

        /// <summary>Use this endpoint to get the spray applications that are associated with a field crop pest.
        /// </summary>
        /// <remarks>The FieldCropPestId must be associated to the Field</remarks>
        [ProducesResponseType(typeof(IEnumerable<FieldSprayApplicationDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Produces(MediaTypeNames.Application.Json)]
        [HttpGet("", Name = "api.spray.get.all")]
        [HttpHead]
        // GET: api/fields/1/sprayapplications
        public async Task<IActionResult> Get(
            [FromRoute] Guid fieldId,
            [FromQuery] FieldSprayResourceParameter resourceParameter,
            [FromHeader(Name = "Accept")] string mediaType)
        {
            var response = await this.businessLogic.GetFieldSprays(fieldId, resourceParameter, mediaType);

            if (!response.IsSuccessful)
                return response.RequestResult;

            Response.Headers.Append("X-Pagination",
                JsonSerializer.Serialize(response.Result.PaginationMetaData));

            return Ok(new
            {
                value = response.Result.Value,
                links = response.Result.Links
            });
        }

        /// <summary>Use this endpoint to get a spray application by id.</summary>
        /// <remarks>The Id must be part of a FieldCropPest associated to the field</remarks>
        [ProducesResponseType(typeof(FieldSprayApplicationDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Produces(MediaTypeNames.Application.Json)]
        [HttpGet("{id:guid}", Name = "api.spray.get.spraybyid")]
        // GET:  api/fields/1/sprayapplications/1
        public IActionResult GetFieldSprayById(
            [FromRoute] Guid fieldId, Guid id,
            [FromHeader(Name = "Accept")] string mediaType)
        {
            var response = this.businessLogic.GetFieldSprayDto(id, mediaType, HttpContext);

            if (!response.IsSuccessful)
                return response.RequestResult;

            return Ok(response.Result);
        }

        /// <summary>Use this end point to add a new spray application to an specific fieldCropPest of a field.</summary>
        /// <remarks>The FieldCropPestId must be associated to the Field</remarks>
        [Consumes(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Produces(MediaTypeNames.Application.Json)]
        [HttpPost("", Name = "api.spray.post.spray")]
        // POST: api/fields/1/sprayapplications
        public async Task<IActionResult> Post(
            [FromRoute] Guid fieldId,
            [FromBody] FieldSprayApplicationForCreationDto sprayForCreationDto,
            [FromHeader(Name = "Accept")] string mediaType)
        {
            var response = await this.businessLogic.AddNewFieldSpray(sprayForCreationDto, HttpContext, mediaType);

            if (!response.IsSuccessful)
                return response.RequestResult;

            return CreatedAtRoute(
                "api.spray.get.spraybyid",
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
        //OPTIONS: api/fields/1/sprayapplications
        public IActionResult Options([FromRoute] Guid fieldId)
        {
            Response.Headers.Append("Allow", "OPTIONS, GET, POST, DELETE");
            return Ok();
        }
    }
}