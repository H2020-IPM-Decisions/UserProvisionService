using H2020.IPMDecisions.UPR.API.Filters;
using H2020.IPMDecisions.UPR.BLL;
using H2020.IPMDecisions.UPR.Core.Dtos;
using H2020.IPMDecisions.UPR.Core.Entities;
using H2020.IPMDecisions.UPR.Core.PatchOperationExamples;
using H2020.IPMDecisions.UPR.Core.ResourceParameters;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.JsonPatch.Operations;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Filters;
using System;
using System.Collections.Generic;
using System.Net.Mime;
using System.Text.Json;
using System.Threading.Tasks;

namespace H2020.IPMDecisions.UPR.API.Controllers
{
    /// <summary>
    /// Use these endpoints to manage the farms associated to a user.
    /// <para>The user will be identified using the UserId on the authentification JWT.</para>
    /// </summary>
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

        /// <summary>Use this endpoint to remove a farm by <paramref name="farmId"/></summary>
        /// <param name="farmId">GUID with the farm Id.</param>
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

        /// <summary>Use this end point to add a new farm to a user.</summary>
        /// <remarks>To receive associated data or HATEOAS links change the 'Accept' header</remarks>
        [ProducesResponseType(typeof(IEnumerable<FarmWithChildrenDto>), StatusCodes.Status200OK)]
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

        /// <summary>Use this endpoint to get a farm by <paramref name="farmId"/></summary>
        /// <param name="farmId">GUID with the farm Id.</param>
        /// <param name="resourceParameter">Parameter to manage resources returned</param>
        /// <param name="mediaType">To receive associated data or HATEOAS links change the 'Accept' header</param>
        [ServiceFilter(typeof(FarmBelongsToUserActionFilter), Order = 2)]
        [ProducesResponseType(typeof(FarmWithChildrenDto), StatusCodes.Status200OK)]
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

        /// <summary>Use this end point to add a new farm to a user.</summary>
        /// <remarks>To receive associated data or HATEOAS links change the 'Accept' header.
        /// <para>Please notice that the creation of Forecast and Historical Weather Data Sources is an important operation as it uses for an overnight schedule operation. </para>
        /// <para>The creation of the weather data sources will change once the weather microservice implements the weatherId.</para>
        /// <para>In a future release only the weatherId will be needed, as the rest of information will be taken from the weather microservice.</para>
        /// </remarks>
        [Consumes(MediaTypeNames.Application.Json)]
        [ProducesResponseType(typeof(FarmDto), StatusCodes.Status201Created)]
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

        /// <summary>Use this endpoint to make a partial update of a farm.</summary>
        /// <remarks>Use the documentation for the FarmFields to manage associated fields to a farm
        /// <para>The forecat and historical data update will change one the weather forecast microservice implements the weatherId.</para>
        /// <para>In a future release only the weatherId will be needed for updating, as the rest of information will be taken from the weather microservice.</para>
        /// </remarks>
        [ServiceFilter(typeof(FarmBelongsToUserActionFilter), Order = 2)]
        [Consumes("application/json-patch+json")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpPatch("{farmId:guid}", Name = "api.farm.patch.farmbyid")]
        [SwaggerRequestExample(typeof(Operation[]), typeof(JsonPatchFarmRequestExample))]
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

            var response = await this.businessLogic.UpdateFarm(farm, farmToPatch, patchDocument);
            if (!response.IsSuccessful)
                return BadRequest(new { message = response.ErrorMessage });

            return NoContent();
        }

        /// <summary>Use this endpoint to make a full update of a farm.</summary>
        /// <remarks>Use the documentation for the FarmFields to manage associated fields to a farm
        /// </remarks>
        [ServiceFilter(typeof(FarmBelongsToUserActionFilter), Order = 2)]
        [Consumes(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpPut("{farmId:guid}", Name = "api.farm.put.farmbyid")]
        //PUT: api/farms/1
        public async Task<IActionResult> FullUpdate(
            [FromRoute] Guid farmId,
            FarmForFullUpdateDto farmForFullUpdate)
        {
            var farm = HttpContext.Items["farm"] as Farm;

            var response = await this.businessLogic.FullUpdateFarm(farm, farmForFullUpdate);
            if (!response.IsSuccessful)
                return BadRequest(new { message = response.ErrorMessage });

            return NoContent();
        }

        /// <summary>Requests permitted on this URL</summary>
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