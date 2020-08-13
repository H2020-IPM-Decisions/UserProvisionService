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
    [Route("api/datashare")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [ServiceFilter(typeof(AddUserIdToContextFilter))]
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
        public async Task<IActionResult> Delete([FromRoute] Guid id)
        {
            var userId = Guid.Parse(HttpContext.Items["userId"].ToString());
            var response = await this.businessLogic.DeleteDataShareRequest(id, userId);

            if (!response.IsSuccessful)
                return BadRequest(new { message = response.ErrorMessage });

            return NoContent();
        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Produces(MediaTypeNames.Application.Json)]
        [HttpGet("", Name = "api.datashare.get.all")]
        [HttpHead]
        // GET: api/datashare
        public async Task<IActionResult> GetAsync(
            [FromQuery] DataShareResourceParameter resourceParameter)
        {
            var userId = Guid.Parse(HttpContext.Items["userId"].ToString());
            var response = await this.businessLogic.GetDataShareRequests(userId, resourceParameter);

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

        [Consumes(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Produces(MediaTypeNames.Application.Json)]
        [Authorize(Policy = "Advisor")]
        [HttpPost("", Name = "api.datashare.post.datashare")]
        // POST: api/datashare
        public async Task<IActionResult> Post(
            [FromBody] DataShareRequestForCreationDto dataShareRequestDto)
        {
            var userId = Guid.Parse(HttpContext.Items["userId"].ToString());
            var response = await this.businessLogic.AddDataShareRequest(userId, dataShareRequestDto);
            
            if (!response.IsSuccessful)
                return BadRequest(new { message = response.ErrorMessage });
            return Ok();
        }

        [Consumes(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Produces(MediaTypeNames.Application.Json)]
        [Authorize(Policy = "Farmer")]
        [HttpPost("Reply", Name = "api.datashare.post.datasharereply")]
        // POST: api/datashare/reply
        public async Task<IActionResult> PostReplyAsync(
            [FromBody] DataShareRequestReplyDto dataShareRequestDto)
        {
            var userId = Guid.Parse(HttpContext.Items["userId"].ToString());
            var response = await this.businessLogic.ReplyToDataShareRequest(userId, dataShareRequestDto);

            if (!response.IsSuccessful)
                return BadRequest(new { message = response.ErrorMessage });
            return Ok();
        }

        [Consumes(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Produces(MediaTypeNames.Application.Json)]
        [Authorize(Policy = "Farmer")]
        [HttpPost("Update", Name = "api.datashare.post.datashareupdate")]
        // POST: api/datashare/update
        public async Task<IActionResult> PostUpdateAsync(
            [FromBody] DataShareRequestUpdateDto dataShareRequestDto)
        {
            var userId = Guid.Parse(HttpContext.Items["userId"].ToString());
            var response = await this.businessLogic.UpdateDataShareRequest(userId, dataShareRequestDto);

            if (!response.IsSuccessful)
                return BadRequest(new { message = response.ErrorMessage });
            return Ok();
        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [HttpOptions]
        //OPTIONS: api/datashare
        public IActionResult Options()
        {
            Response.Headers.Add("Allow", "OPTIONS, GET, POST");
            return Ok();
        }
    }
}