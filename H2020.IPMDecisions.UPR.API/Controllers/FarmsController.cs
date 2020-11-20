using H2020.IPMDecisions.UPR.API.Filters;
using H2020.IPMDecisions.UPR.BLL;
using H2020.IPMDecisions.UPR.Core.Dtos;
using H2020.IPMDecisions.UPR.Core.Entities;
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
    [ServiceFilter(typeof(AddUserIdToContextFilter), Order = 1)]
    public class FarmsController : ControllerBase
    {
        private readonly IBusinessLogic businessLogic;

        public FarmsController(IBusinessLogic businessLogic)
        {
            this.businessLogic = businessLogic
                ?? throw new System.ArgumentNullException(nameof(businessLogic));
        }

        [ServiceFilter(typeof(FarmBelongsToUserActionFilter), Order = 2)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpDelete("{farmId:guid}", Name = "api.farm.delete.farmbyid")]
        //DELETE: api/farms/1
        public async Task<IActionResult> Delete([FromRoute] Guid farmId)
        {
            var response = await this.businessLogic.DeleteFarm(HttpContext);

            if (!response.IsSuccessful)
                return BadRequest(new { message = response.ErrorMessage });

            return NoContent();
        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Produces(MediaTypeNames.Application.Json,
            "application/vnd.h2020ipmdecisions.hateoas+json",
            "application/vnd.h2020ipmdecisions.farm.withchildren+json",
            "application/vnd.h2020ipmdecisions.farm.withchildren.hateoas+json")]
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

        [ServiceFilter(typeof(FarmBelongsToUserActionFilter), Order = 2)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Produces(MediaTypeNames.Application.Json,
            "application/vnd.h2020ipmdecisions.hateoas+json",
            "application/vnd.h2020ipmdecisions.farm.withchildren+json",
            "application/vnd.h2020ipmdecisions.farm.withchildren.hateoas+json")]
        [HttpGet("{farmId:guid}", Name = "api.farm.get.farmbyid")]
        // GET:  api/farms/1
        public IActionResult Get([FromRoute] Guid farmId,
            [FromQuery] FarmResourceParameter resourceParameter,
            [FromHeader(Name = "Accept")] string mediaType)
        {
            var response = this.businessLogic.GetFarmDto(farmId, HttpContext, resourceParameter, mediaType);

            if (!response.IsSuccessful)
                return response.RequestResult;

            return Ok(response.Result);
        }

        [Consumes(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Produces(MediaTypeNames.Application.Json,
            "application/vnd.h2020ipmdecisions.hateoas+json")]
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
                new { farmId = response.Result["Id"] },
                response.Result);
        }

        [ServiceFilter(typeof(FarmBelongsToUserActionFilter), Order = 2)]
        [Consumes("application/json-patch+json")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpPatch("{farmId:guid}", Name = "api.farm.patch.farmbyid")]
        //PATCH: api/farms/1
        public async Task<IActionResult> PartialUpdate(
            [FromRoute] Guid farmId,
            JsonPatchDocument<FarmForUpdateDto> patchDocument)
        {
            var farm = HttpContext.Items["farm"] as Farm;

            FarmForUpdateDto farmToPatch =
                this.businessLogic.MapToFarmForUpdateDto(farm);
            patchDocument.ApplyTo(farmToPatch, ModelState);
            if (!TryValidateModel(farmToPatch))
                return ValidationProblem(ModelState);

            var response = await this.businessLogic.UpdateFarm(farm, farmToPatch);
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