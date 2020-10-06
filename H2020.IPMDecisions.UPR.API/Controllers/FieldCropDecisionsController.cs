using System;
using System.Net.Mime;
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
    [Route("api/fields/{fieldId:guid}/cropdecisions")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [ServiceFilter(typeof(AddUserIdToContextFilter), Order = 1)]
    [ServiceFilter(typeof(FieldBelongsToUserActionFilter), Order = 2)]
    public class FieldCropDecisionsController : ControllerBase
    {
        private readonly IBusinessLogic businessLogic;
        public FieldCropDecisionsController(IBusinessLogic businessLogic)
        {
            this.businessLogic = businessLogic
                ?? throw new System.ArgumentNullException(nameof(businessLogic));
        }

        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpDelete("{id:guid}", Name = "api.fieldcropdecisions.delete.cropdecisionbyid")]
        //DELETE: api/fields/1/cropdecisions/1
        public IActionResult Delete(
            [FromRoute] Guid fieldId, Guid id)
        {
            throw new NotImplementedException();
        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Produces(MediaTypeNames.Application.Json)]
        [HttpGet("", Name = "api.fieldcropdecisions.get.all")]
        [HttpHead]
        // GET: api/fields/1/cropdecisions
        public IActionResult Get(
            [FromRoute] Guid fieldId,
            [FromQuery] FieldCropPestResourceParameter resourceParameter,
            [FromHeader(Name = "Accept")] string mediaType)
        {
            throw new NotImplementedException();
        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Produces(MediaTypeNames.Application.Json)]
        [HttpGet("{id:guid}", Name = "api.fieldcropdecisions.get.cropdecisionbyid")]
        // GET:  api/fields/1/cropdecisions/1
        public IActionResult GetById(
            [FromRoute] Guid fieldId, Guid id,
            [FromHeader(Name = "Accept")] string mediaType)
        {
            throw new NotImplementedException();
        }

        [Consumes(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [Produces(MediaTypeNames.Application.Json)]
        [HttpPost("", Name = "api.fieldcropdecisions.post.cropdecision")]
        // POST: api/fields/1/cropdecisions
        public async Task<IActionResult> Post(
            [FromRoute] Guid fieldId,
            [FromBody] CropPestDssForCreationDto cropPestDssForCreationDto,
            [FromHeader(Name = "Accept")] string mediaType)
        {
            var response = await this.businessLogic.AddNewFieldCropDecision(cropPestDssForCreationDto, HttpContext, mediaType);
            if (!response.IsSuccessful)
                return response.RequestResult;
                           
            return CreatedAtRoute(
                "api.fieldcropdecisions.get.cropdecisionbyid",
                new
                {
                    fieldId,
                    id = response.Result["Id"]
                },
                response.Result);
        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [HttpOptions]
        //OPTIONS: api/fields/1/cropdecisions
        public IActionResult Options([FromRoute] Guid fieldId)
        {
            Response.Headers.Add("Allow", "OPTIONS, GET, POST, DELETE");
            return Ok();
        }
    }
}