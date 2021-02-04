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
using System.Text.Json;
using System.Collections.Generic;
using H2020.IPMDecisions.UPR.Core.PatchOperationExamples;
using Swashbuckle.AspNetCore.Filters;
using Microsoft.AspNetCore.JsonPatch.Operations;

namespace H2020.IPMDecisions.UPR.API.Controllers
{
    /// <summary>
    /// These endpoints allows to manage Fields associated to a farm.
    /// <para>The FarmId on the URL must be associated to the UserId of the Authorization JWT.</para>
    /// </summary>
    [ApiController]
    [Route("api/farms/{farmId:guid}/fields")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [ServiceFilter(typeof(AddUserIdToContextFilter), Order = 1)]
    [ServiceFilter(typeof(FarmBelongsToUserActionFilter), Order = 2)]
    public class FarmFieldsController : ControllerBase
    {
        private readonly IBusinessLogic businessLogic;
        public FarmFieldsController(IBusinessLogic businessLogic)
        {
            this.businessLogic = businessLogic
                ?? throw new System.ArgumentNullException(nameof(businessLogic));
        }

        /// <summary>Deletes a field by <paramref name="id"/></summary>
        /// <param name="farmId">GUID with the farm Id.</param>
        /// <param name="id">GUID with field Id.</param>
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpDelete("{id:guid}", Name = "api.field.delete.fieldbyid")]
        //DELETE: api/farms/1/fields/1
        public async Task<IActionResult> Delete(
            [FromRoute] Guid farmId, Guid id)
        {
            var response = await this.businessLogic.DeleteField(id);

            if (!response.IsSuccessful)
                return BadRequest(new { message = response.ErrorMessage });

            return NoContent();
        }

        /// <summary>Use this endpoint to get the fields that are associated with a farm.</summary>
        /// <remark>To receive associated data or HATEOAS links change the 'Accept' header.</remark>
        [ProducesResponseType(typeof(IEnumerable<FieldWithChildrenDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Produces(MediaTypeNames.Application.Json,
            "application/vnd.h2020ipmdecisions.hateoas+json",
            "application/vnd.h2020ipmdecisions.field.withchildren+json",
            "application/vnd.h2020ipmdecisions.field.withchildren.hateoas+json")]
        [HttpGet("", Name = "api.field.get.all")]
        [HttpHead]
        // GET: api/farms/1/fields
        public async Task<IActionResult> Get(
            [FromRoute] Guid farmId,
            [FromQuery] FieldResourceParameter resourceParameter,
            [FromHeader(Name = "Accept")] string mediaType)
        {
            var response = await this.businessLogic.GetFields(farmId, resourceParameter, mediaType);

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

        /// <summary>Use this endpoint to get an unique field that is associated with a farm.</summary>
        /// <remark>To receive associated data or HATEOAS links change the 'Accept' header</remark>
        [ProducesResponseType(typeof(FieldWithChildrenDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Produces(MediaTypeNames.Application.Json,
            "application/vnd.h2020ipmdecisions.hateoas+json",
            "application/vnd.h2020ipmdecisions.field.withchildren+json",
            "application/vnd.h2020ipmdecisions.field.withchildren.hateoas+json")]
        [HttpGet("{id:guid}", Name = "api.field.get.fieldbyid")]
        // GET:  api/farms/1/fields/1
        public async Task<IActionResult> GetFieldById(
            [FromRoute] Guid farmId, Guid id,
            [FromQuery] FieldResourceParameter resourceParameter,
            [FromHeader(Name = "Accept")] string mediaType)
        {
            var response = await this.businessLogic.GetFieldDto(id, resourceParameter, mediaType);

            if (!response.IsSuccessful)
                return response.RequestResult;

            return Ok(response.Result);
        }

        /// <summary>Use this end point to add a new field to a farm.</summary>
        /// <remark>To receive associated data or HATEOAS links change the 'Accept' header</remark>
        [Consumes(MediaTypeNames.Application.Json)]
        [ProducesResponseType(typeof(FieldDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Produces(MediaTypeNames.Application.Json,
            "application/vnd.h2020ipmdecisions.hateoas+json",
            "application/vnd.h2020ipmdecisions.field.withchildren+json",
            "application/vnd.h2020ipmdecisions.field.withchildren.hateoas+json")]
        [HttpPost("", Name = "api.field.post.field")]
        // POST: api/farms/1/fields
        public async Task<IActionResult> Post(
            [FromRoute] Guid farmId,
            [FromBody] FieldForCreationDto fieldForCreationDto,
            [FromHeader(Name = "Accept")] string mediaType)
        {
            var response = await this.businessLogic.AddNewField(fieldForCreationDto, HttpContext, mediaType);

            if (!response.IsSuccessful)
                return response.RequestResult;

            return CreatedAtRoute(
                "api.field.get.fieldbyid",
                new
                {
                    farmId,
                    id = response.Result["Id"]
                },
                response.Result);
        }

        /// <summary>Use this endpoint to make a partial update of a field.</summary>
        /// <remarks>Any property for a field will be updated as usual PATCH conventions. Only the FieldCropPestDto parameter are manage different as the update needs to identify exactly the record.
        /// For this reason the FieldCropPest expects the ID on the path instead the array location as explained on PATCH conventions.
        /// Please see examples below:
        /// <para>To remove a record: use the "remove" operation and include the fieldCropPestId on the path parameter "/fieldCropDto/fieldCropPestDto/{fieldCropPestId}</para>
        /// <para>To create a new record: use the "add" operation and a pest EPPO code on the value parameter. Use the path parameter without and id "/fieldCropDto/fieldCropPestDto"</para>
        /// <para>To replace a record: use the "replace" operation. Include the fieldCropPestId on the path parameter,"/fieldCropDto/fieldCropPestDto/{fieldCropPestId}, and a pest EPPO code on the value parameter.</para>
        /// <para>For an example payload, please see the 'Request body' section.</para>
        /// </remarks>
        [Consumes("application/json-patch+json")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpPatch("{id:guid}", Name = "api.field.patch.fieldbyid")]
        [SwaggerRequestExample(typeof(Operation), typeof(JsonPatchFieldRequestExample))]
        //PATCH: api/farms/1/fields/1
        public async Task<IActionResult> PartialUpdate(
            [FromRoute] Guid farmId, Guid id,
            JsonPatchDocument<FieldForUpdateDto> patchDocument)
        {
            var fieldResponse = await this.businessLogic.GetField(id, HttpContext);
            if (!fieldResponse.IsSuccessful)
                return fieldResponse.RequestResult;

            FieldForUpdateDto fieldToPatch =
                this.businessLogic.MapToFieldForUpdateDto(fieldResponse.Result);
            patchDocument.ApplyTo(fieldToPatch, ModelState);

            var response = await this.businessLogic.UpdateField(fieldResponse.Result, fieldToPatch, patchDocument);
            if (!response.IsSuccessful)
                return response.RequestResult;
            return NoContent();
        }

        /// <summary>Requests permitted on this URL</summary>
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