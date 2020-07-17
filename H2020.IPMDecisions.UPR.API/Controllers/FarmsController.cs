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
using H2020.IPMDecisions.UPR.API.Filters;
using System.Text.Json;

namespace H2020.IPMDecisions.UPR.API.Controllers
{
    [ApiController]
    [Route("api/farms")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [ServiceFilter(typeof(AddUserIdToContextFilter))]
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
            var response = await this.businessLogic.DeleteFarm(id, HttpContext);

            if (!response.IsSuccessful)
                return BadRequest(new { message = response.ErrorMessage });

            return NoContent();
        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Produces(MediaTypeNames.Application.Json)]
        [HttpGet("", Name = "GetFarms")]
        [HttpHead]
        // GET: api/farms
        public async Task<IActionResult> Get(
            [FromQuery] FarmResourceParameter resourceParameter,
            [FromHeader(Name = "Accept")] string mediaType)
        {
            var userId = Guid.Parse(HttpContext.Items["userId"].ToString());
            var response = await this.businessLogic.GetFarms(userId, resourceParameter, mediaType);

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

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Produces(MediaTypeNames.Application.Json)]
        [HttpGet("{id:guid}", Name = "GetFarmById")]
        [HttpHead]
        // GET:  api/farms/1
        public async Task<IActionResult> GetFarmById([FromRoute] Guid id,
            [FromQuery] string fields,
            [FromHeader(Name = "Accept")] string mediaType)
        {
            var response = await this.businessLogic.GetFarmById(id, fields, HttpContext, mediaType);

            if (!response.IsSuccessful)
                return response.RequestResult;

            return Ok(response.Result);
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
            var userId = Guid.Parse(HttpContext.Items["userId"].ToString());
            var response = await this.businessLogic.LinkNewFarmToUserProfile(farmForCreationDto, userId, mediaType);

            if (!response.IsSuccessful)
                return response.RequestResult;
            return CreatedAtRoute(
                "GetFarmById",
                new { id = response.Result.Id },
                response.Result);
        }

        // [Consumes("application/json-patch+json")]
        // [ProducesResponseType(StatusCodes.Status201Created)]
        // [ProducesResponseType(StatusCodes.Status204NoContent)]
        // [ProducesResponseType(StatusCodes.Status400BadRequest)]
        // [HttpPatch("{id:guid}", Name = "PartialUpdateFarm")]
        // //PATCH: api/farms/1
        // public async Task<IActionResult> PartialUpdate(
        //     [FromRoute] Guid id,
        //     JsonPatchDocument<FarmForUpdateDto> patchDocument)
        // {
        //     throw new NotImplementedException();
        // }

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