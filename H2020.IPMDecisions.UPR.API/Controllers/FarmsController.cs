using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using H2020.IPMDecisions.UPR.BLL;
using System.Net.Mime;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using H2020.IPMDecisions.UPR.Core.Dtos;
using System;
using H2020.IPMDecisions.UPR.Core.ResourceParameters;
using Microsoft.AspNetCore.JsonPatch;

namespace H2020.IPMDecisions.UPR.API.Controllers
{
    [ApiController]
    [Route("api/farms")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class FarmsController : ControllerBase
    {
        private readonly IBusinessLogic businessLogic;

        public FarmsController(IBusinessLogic businessLogic)
        {
            this.businessLogic = businessLogic
                ?? throw new System.ArgumentNullException(nameof(businessLogic));
        }

        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpDelete("{id:guid}", Name = "DeleteFarm")]
        //DELETE: api/farms/1
        public async Task<IActionResult> Delete([FromRoute] Guid id)
        {
            throw new NotImplementedException();
        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Produces(MediaTypeNames.Application.Json)]
        [HttpGet("", Name = "GetFarms")]
        [HttpHead]
        // GET: api/famrs
        public async Task<IActionResult> GetFarms(
            [FromQuery] FarmResourceParameter resourceParameter,
            [FromHeader(Name = "Accept")] string mediaType)
        {
            throw new NotImplementedException();
        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Produces(MediaTypeNames.Application.Json)]
        [HttpGet("{id:guid}", Name = "GetFarmById")]
        [HttpHead]
        // GET:  api/farms/1
        public async Task<IActionResult> Get([FromRoute] Guid id,
            [FromQuery] string fields,
            [FromHeader(Name = "Accept")] string mediaType)
        {
            throw new NotImplementedException();
        }

        [Consumes(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Produces(MediaTypeNames.Application.Json)]
        [HttpPost("", Name = "CreateFarm")]
        // POST: api/farms
        public async Task<IActionResult> Post(
            [FromBody] FarmForCreationDto farmForCreationDto,
            [FromHeader(Name = "Accept")] string mediaType)
        {
            throw new NotImplementedException();
        }

        [Consumes("application/json-patch+json")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpPatch("{id:guid}", Name = "PartialUpdateFarm")]
        //PATCH: api/farms/1
        public async Task<IActionResult> PartialUpdate(
            [FromRoute] Guid id,
            JsonPatchDocument<FarmForUpdateDto> patchDocument)
        {
            throw new NotImplementedException();
        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [HttpOptions]
        //OPTIONS: api/farms
        public IActionResult Options()
        {
            Response.Headers.Add("Allow", "OPTIONS, GET, PATCH, POST, DELETE");
            return Ok();
        }
    }
}