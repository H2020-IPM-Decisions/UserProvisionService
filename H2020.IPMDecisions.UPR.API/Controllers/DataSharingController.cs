using System;
using System.Net;
using System.Net.Mime;
using System.Threading.Tasks;
using H2020.IPMDecisions.UPR.BLL;
using H2020.IPMDecisions.UPR.Core.Dtos;
using H2020.IPMDecisions.UPR.Core.ResourceParameters;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;

namespace H2020.IPMDecisions.UPR.API.Controllers
{
    [ApiController]
    [Route("api/datashare")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class DataSharingController : ControllerBase
    {
        private readonly IBusinessLogic businessLogic;
        public DataSharingController(IBusinessLogic businessLogic)
        {
            this.businessLogic = businessLogic
               ?? throw new System.ArgumentNullException(nameof(businessLogic));
        }

        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpDelete("{id:guid}", Name = "api.datashare.delete.id")]
        //DELETE: api/datashare/1
        public IActionResult Delete(
            [FromRoute] Guid id)
        {
            return NoContent();
        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Produces(MediaTypeNames.Application.Json)]
        [HttpGet("", Name = "api.datashare.get.all")]
        [HttpHead]
        // GET: api/datashare
        public IActionResult Get(
            [FromQuery] Object resourceParameter,
            [FromHeader(Name = "Accept")] string mediaType)
        {
            return Ok();
        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Produces(MediaTypeNames.Application.Json)]
        [HttpGet("{id:guid}", Name = "api.datashare.get.id")]
        // GET:  api/datashare/1
        public IActionResult GetDataShareById(
            [FromRoute] Guid id,
            [FromQuery] BaseResourceParameter resourceParameter,
            [FromHeader(Name = "Accept")] string mediaType)
        {
            return Ok();
        }

        [Consumes(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Produces(MediaTypeNames.Application.Json)]
        [HttpPost("", Name = "api.datashare.post.datashare")]
        // POST: api/datashare
        public IActionResult Post(
            [FromBody] Object dataShareRequestDto,
            [FromHeader(Name = "Accept")] string mediaType)
        {
            return Ok();
        }

        [Consumes("application/json-patch+json")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpPatch("{id:guid}", Name = "api.datashare.patch.id")]
        //PATCH: api/datashare/1
        public IActionResult PartialUpdate(
            [FromRoute] Guid id,
            JsonPatchDocument<Object> patchDocument)
        {
            return Ok();
        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [HttpOptions]
        //OPTIONS: api/datashare
        public IActionResult Options()
        {
            Response.Headers.Add("Allow", "OPTIONS, GET, PATCH, POST, DELETE");
            return Ok();
        }
    }
}