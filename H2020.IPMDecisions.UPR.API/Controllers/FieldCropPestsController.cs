using System;
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
    [ApiController]
    [Route("api/fields/{fieldId:guid}/croppests")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [ServiceFilter(typeof(AddUserIdToContextFilter), Order = 1)]
    [ServiceFilter(typeof(FieldBelongsToUserActionFilter), Order = 2)]
    public class FieldCropPestsController : ControllerBase
    {
        private readonly IBusinessLogic businessLogic;
        public FieldCropPestsController(IBusinessLogic businessLogic)
        {
            this.businessLogic = businessLogic
                ?? throw new System.ArgumentNullException(nameof(businessLogic));
        }

        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpDelete("{id:guid}", Name = "api.fieldcroppests.delete.croppestbyid")]
        //DELETE: api/fields/1/croppests/1
        public async Task<IActionResult> Delete(
            [FromRoute] Guid fieldId, Guid id)
        {
            var response = await this.businessLogic.DeleteFieldCropPest(id);

            if (!response.IsSuccessful)
                return BadRequest(new { message = response.ErrorMessage });

            return NoContent();
        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Produces(MediaTypeNames.Application.Json)]
        [HttpGet("", Name = "api.fieldcroppests.get.all")]
        [HttpHead]
        // GET: api/fields/1/croppests
        public async Task<IActionResult> Get(
            [FromRoute] Guid fieldId,
            [FromQuery] FieldCropPestResourceParameter resourceParameter,
            [FromHeader(Name = "Accept")] string mediaType)
        {
            var response = await this.businessLogic.GetFieldCropPests(fieldId, resourceParameter, mediaType);

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
        [HttpGet("{id:guid}", Name = "api.fieldcroppests.get.croppestbyid")]
        // GET:  api/fields/1/croppests/1
        public async Task<IActionResult> GetById(
            [FromRoute] Guid fieldId, Guid id,
            [FromHeader(Name = "Accept")] string mediaType)
        {
            var response = await this.businessLogic.GetFieldCropPest(id, mediaType);

            if (!response.IsSuccessful)
                return response.RequestResult;

            return Ok(response.Result);
        }

        [Consumes(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Produces(MediaTypeNames.Application.Json)]
        [HttpPost("", Name = "api.fieldcroppests.post.croppest")]
        // POST: api/fields/1/croppests
        public async Task<IActionResult> Post(
            [FromRoute] Guid fieldId,
            [FromBody] CropPestForCreationDto cropPestForCreationDto,
            [FromHeader(Name = "Accept")] string mediaType)
        {
            var response = await this.businessLogic.AddNewFieldCropPest(cropPestForCreationDto, HttpContext, mediaType);

            if (!response.IsSuccessful)
                return response.RequestResult;

            return CreatedAtRoute(
                "api.fieldcroppests.get.croppestbyid",
                new
                {
                    fieldId,
                    id = response.Result.Id
                },
                response.Result);
        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [HttpOptions]
        //OPTIONS: api/fields/1/croppests
        public IActionResult Options([FromRoute] Guid fieldId)
        {
            Response.Headers.Add("Allow", "OPTIONS, GET, POST, DELETE");
            return Ok();
        }
    }
}