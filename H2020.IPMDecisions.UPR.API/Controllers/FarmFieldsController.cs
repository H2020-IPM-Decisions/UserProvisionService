using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using H2020.IPMDecisions.UPR.BLL;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using System;
using System.Net.Mime;
using H2020.IPMDecisions.UPR.Core.Dtos;
using Microsoft.AspNetCore.JsonPatch;
using H2020.IPMDecisions.UPR.Core.ResourceParameters;
using H2020.IPMDecisions.UPR.API.Filters;
using H2020.IPMDecisions.UPR.Core.Entities;

namespace H2020.IPMDecisions.UPR.API.Controllers
{
    [ApiController]
    [Route("api/farms/{farmId:guid}/fields")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [ServiceFilter(typeof(AddUserIdToContextFilter), Order=1)]
    [ServiceFilter(typeof(FarmBelongsToUserActionFilter), Order=2)]
    public class FarmFieldsController : ControllerBase
    {
        private readonly IBusinessLogic businessLogic;
        public FarmFieldsController(IBusinessLogic businessLogic)
        {
            this.businessLogic = businessLogic
                ?? throw new System.ArgumentNullException(nameof(businessLogic));
        }

        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpDelete("{id:guid}", Name = "DeleteField")]
        //DELETE: api/farms/1/fields/1
        public async Task<IActionResult> Delete(
            [FromRoute] Guid farmId, Guid id)
        {
            throw new NotImplementedException();
        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Produces(MediaTypeNames.Application.Json)]
        [HttpGet("", Name = "Getfields")]
        [HttpHead]
        // GET: api/farms/1/fields
        public async Task<IActionResult> GetFields(
            [FromRoute] Guid farmId,
            [FromQuery] FieldResourceParameter resourceParameter,
            [FromHeader(Name = "Accept")] string mediaType)
        {
            throw new NotImplementedException();
        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Produces(MediaTypeNames.Application.Json)]
        [HttpGet("{id:guid}", Name = "GetFieldById")]
        [HttpHead]
        // GET:  api/farms/1/fields/1
        public async Task<IActionResult> Get(
            [FromRoute] Guid farmId, Guid id,
            [FromQuery] string fields,
            [FromHeader(Name = "Accept")] string mediaType)
        {
            throw new NotImplementedException();
        }

        [Consumes(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Produces(MediaTypeNames.Application.Json)]
        [HttpPost("", Name = "CreateField")]
        // POST: api/farms/1/fields
        public async Task<IActionResult> Post(
            [FromRoute] Guid farmId,
            [FromBody] FieldForCreationDto fieldForCreationDto,
            [FromHeader(Name = "Accept")] string mediaType)
        {
            var response = await this.businessLogic.LinkNewFieldToFarm(fieldForCreationDto, HttpContext, mediaType);

            if (!response.IsSuccessful)
                return response.RequestResult;

                return Ok(response.Result);

            // return CreatedAtRoute(
            //     "GetFieldById",
            //     new {
            //         farmId = response.Result.FarmId, 
            //         id = response.Result.Id },
            //     response.Result);
        }

        [Consumes("application/json-patch+json")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpPatch("{id:guid}", Name = "PartialUpdateField")]
        //PATCH: api/farms/1/fields/1
        public async Task<IActionResult> PartialUpdate(
            [FromRoute] Guid farmId, Guid id,
            JsonPatchDocument<FieldForUpdateDto> patchDocument)
        {
            throw new NotImplementedException();
        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [HttpOptions]
        //OPTIONS: api/farms/1/fields
        public IActionResult Options([FromRoute] Guid farmId)
        {
            Response.Headers.Add("Allow", "OPTIONS, GET, PATCH, POST, DELETE");
            return Ok();
        }
    }
}