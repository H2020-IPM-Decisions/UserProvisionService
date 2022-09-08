using System;
using System.Collections.Generic;
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
    /// <summary>
    /// These end points allows users to manage the data shared with other users.
    /// <para>The user will be identified using the UserId on the authentication JWT.</para>
    /// </summary>
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

        /// <summary>Deletes a data share request by <paramref name="id"/></summary>
        /// <remarks>The "Advisor" will loose access to any of the farms associated with this request.
        /// <para>Accessible by users with policy claim "Farmer" and "Advisor".</para>
        /// </remarks>
        /// <param name="id">GUID with data share request Id.</param>
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
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

        /// <summary>Get all the data share requests from a user.</summary>
        /// <remarks>Accessible by users with policy claim "Farmer" and "Advisor".</remarks>
        [ProducesResponseType(typeof(IEnumerable<DataShareRequestDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
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

        /// <summary>This end point sends a request to another user to share data.</summary>
        /// <remarks>
        /// <para>Accessible by users with policy claim "Advisor".</para>
        /// </remarks>
        [Consumes(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Produces(MediaTypeNames.Application.Json)]
        [Authorize(Policy = "advisor")]
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

        /// <summary>
        /// Use this end point to accept or decline the data share request make by an Advisor.       
        /// </summary> 
        /// <remarks>
        /// A user with claim "Farmer" can accept or decline a request done by
        /// a user with claim "Advisor".
        /// <para>Valid Reply options are: Accepted or Declined</para>
        /// <para>Accessible by users with policy claim "Farmer".</para>
        /// </remarks>
        [Consumes(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Produces(MediaTypeNames.Application.Json)]
        [Authorize(Policy = "farmer")]
        [HttpPost("reply", Name = "api.datashare.post.datasharereply")]
        // POST: api/datashare/reply
        public async Task<IActionResult> PostReplyAsync(
            [FromBody] DataShareRequestReplyDto dataShareRequestDto)
        {
            var userId = Guid.Parse(HttpContext.Items["userId"].ToString());
            var response = await this.businessLogic.ReplyToDataShareRequest(userId, dataShareRequestDto);

            if (!response.IsSuccessful)
                return BadRequest(new { message = response.ErrorMessage });
            return NoContent();
        }

        /// <summary>
        /// This end point, will update the status (accepted or declined) from a data share request.
        ///</summary>
        /// <remarks>
        /// <para>If "Reply" status changes to 'Declined', the advisor will loose access to all the farms.</para>
        /// <para>If "Reply" status changes or keeps the 'Accepted' status, the user have the option to select which farms are accessible by the Advisor.</para>
        /// <para>Accessible by users with policy claim "Farmer".</para>
        /// </remarks>
        [Consumes(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Produces(MediaTypeNames.Application.Json)]
        [Authorize(Policy = "farmer")]
        [HttpPost("update", Name = "api.datashare.post.datashareupdate")]
        // POST: api/datashare/update
        public async Task<IActionResult> PostUpdateAsync(
            [FromBody] DataShareRequestUpdateDto dataShareRequestDto)
        {
            var userId = Guid.Parse(HttpContext.Items["userId"].ToString());
            var response = await this.businessLogic.UpdateDataShareRequest(userId, dataShareRequestDto);

            if (!response.IsSuccessful)
                return BadRequest(new { message = response.ErrorMessage });
            return NoContent();
        }

        /// <summary>Requests permitted on this URL</summary>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [HttpOptions]
        //OPTIONS: api/datashare
        public IActionResult Options()
        {
            Response.Headers.Add("Allow", "OPTIONS, GET, POST, DELETE");
            return Ok();
        }
    }
}