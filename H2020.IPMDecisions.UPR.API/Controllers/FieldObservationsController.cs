using System;
using System.Net.Mime;
using System.Threading.Tasks;
using H2020.IPMDecisions.UPR.API.Filters;
using H2020.IPMDecisions.UPR.BLL;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;

namespace H2020.IPMDecisions.UPR.API.Controllers
{
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

        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpDelete("{id:guid}", Name = "DeleteObservation")]
        //DELETE: api/fields/1/observations/1
        public async Task<IActionResult> Delete(
            [FromRoute] Guid fieldId, Guid id)
        {
            return Ok();
        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Produces(MediaTypeNames.Application.Json)]
        [HttpGet("", Name = "GetObservation")]
        [HttpHead]
        // GET: api/fields/1/observations
        public async Task<IActionResult> Get(
            [FromRoute] Guid fieldId,
            [FromQuery] object resourceParameter,
            [FromHeader(Name = "Accept")] string mediaType)
        {
            return Ok();
        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Produces(MediaTypeNames.Application.Json)]
        [HttpGet("{id:guid}", Name = "GetObservationById")]
        [HttpHead]
        // GET:  api/fields/1/observations/1
        public async Task<IActionResult> GetFieldById(
            [FromRoute] Guid fieldId, Guid id,
            [FromQuery] string fields,
            [FromHeader(Name = "Accept")] string mediaType)
        {
            return Ok();
        }

        [Consumes(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Produces(MediaTypeNames.Application.Json)]
        [HttpPost("", Name = "CreateObservation")]
        // POST: api/fields/1/observations
        public async Task<IActionResult> Post(
            [FromRoute] Guid fieldId,
            [FromBody] object observationForCreationDto,
            [FromHeader(Name = "Accept")] string mediaType)
        {
            return Ok();
        }

        [Consumes("application/json-patch+json")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpPatch("{id:guid}", Name = "PartialUpdateObservation")]
        //PATCH: api/fields/1/observations/1
        public async Task<IActionResult> PartialUpdate(
            [FromRoute] Guid fieldId, Guid id,
            JsonPatchDocument<object> patchDocument)
        {
            return Ok();
        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [HttpOptions]
        //OPTIONS: api/fields/1/observations
        public IActionResult Options([FromRoute] Guid fieldId)
        {
            Response.Headers.Add("Allow", "OPTIONS, GET, PATCH, POST, DELETE");
            return Ok();
        }
    }
}