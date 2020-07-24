using H2020.IPMDecisions.UPR.API.Filters;
using H2020.IPMDecisions.UPR.BLL;
using H2020.IPMDecisions.UPR.Core.Dtos;
using H2020.IPMDecisions.UPR.Core.ResourceParameters;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Net.Mime;
using System.Text.Json;
using System.Threading.Tasks;

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
        [HttpDelete("{id:guid}", Name = "api.farm.delete.farmbyid")]
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
        [Produces(MediaTypeNames.Application.Json, "application/vnd.h2020ipmdecisions.hateoas+json")]
        [HttpGet("", Name = "api.farm.get.all")]
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
        [Produces(MediaTypeNames.Application.Json,
            "application/vnd.h2020ipmdecisions.hateoas+json",
            "application/vnd.h2020ipmdecisions.farm.withchildren+json",
            "application/vnd.h2020ipmdecisions.farm.withchildren.hateoas+json")]
        [HttpGet("{id:guid}", Name = "api.farm.get.farmbyid")]
        [HttpHead]
        // GET:  api/farms/1
        public async Task<IActionResult> GetFarmById([FromRoute] Guid id,
            [FromQuery] ChildrenResourceParameter resourceParameter,
            [FromHeader(Name = "Accept")] string mediaType)
        {
            var response = await this.businessLogic.GetFarmDto(id, HttpContext, resourceParameter, mediaType);

            if (!response.IsSuccessful)
                return response.RequestResult;

            return Ok(response.Result);
        }

        [Consumes(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Produces(MediaTypeNames.Application.Json, 
            "application/vnd.h2020ipmdecisions.hateoas+json",
            "application/vnd.h2020ipmdecisions.farm.withchildren+json",
            "application/vnd.h2020ipmdecisions.farm.withchildren.hateoas+json")]
        [HttpPost("", Name = "api.farm.post.farm")]
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
                "api.farm.get.farmbyid",
                new { id = response.Result["Id"] },
                response.Result);
        }

        [Consumes("application/json-patch+json")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpPatch("{id:guid}", Name = "api.farm.patch.farmbyid")]
        //PATCH: api/farms/1
        public async Task<IActionResult> PartialUpdate(
            [FromRoute] Guid id,
            JsonPatchDocument<FarmForUpdateDto> patchDocument)
        {
            var farmResponse = await this.businessLogic.GetFarm(id, HttpContext);
            if (!farmResponse.IsSuccessful)
                return farmResponse.RequestResult;

            FarmForUpdateDto farmToPatch =
                this.businessLogic.MapToFarmForUpdateDto(farmResponse.Result);
            patchDocument.ApplyTo(farmToPatch, ModelState);
            if (!TryValidateModel(farmToPatch))
                return ValidationProblem(ModelState);

            var response = await this.businessLogic.UpdateFarm(farmResponse.Result, farmToPatch);
            if (!response.IsSuccessful)
                return BadRequest(new { message = response.ErrorMessage });

            return NoContent();
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