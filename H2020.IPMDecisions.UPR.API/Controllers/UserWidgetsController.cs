using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System.Net.Mime;
using H2020.IPMDecisions.UPR.BLL;
using H2020.IPMDecisions.UPR.API.Filters;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.Collections.Generic;
using H2020.IPMDecisions.UPR.Core.Dtos;
using Microsoft.AspNetCore.JsonPatch;
using Swashbuckle.AspNetCore.Filters;
using Microsoft.AspNetCore.JsonPatch.Operations;
using H2020.IPMDecisions.UPR.Core.PatchOperationExamples;

namespace H2020.IPMDecisions.UPR.API.Controllers
{
    /// <summary>
    /// These end points to manage the widgets for a user.
    /// <para>The user will be identified using the UserId on the authentication JWT.</para>
    /// </summary>
    [ApiController]
    [Route("api/users/widgets")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [ServiceFilter(typeof(AddUserIdToContextFilter))]
    [ServiceFilter(typeof(UserAccessingOwnDataActionFilter))]
    public class UserWidgetsController : ControllerBase
    {
        private readonly IBusinessLogic businessLogic;

        public UserWidgetsController(IBusinessLogic businessLogic)
        {
            this.businessLogic = businessLogic
                ?? throw new ArgumentNullException(nameof(businessLogic));
        }

        /// <summary>
        /// Returns the list of widgets allowed or not by a user.
        /// </summary>
        [ProducesResponseType(typeof(IEnumerable<UserWidgetDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        [Produces(MediaTypeNames.Application.Json)]
        [HttpGet("", Name = "api.userwidget.get.userid")]
        [HttpHead]
        // GET:  api/users/widgets
        public async Task<IActionResult> Get()
        {
            var userId = Guid.Parse(HttpContext.Items["userId"].ToString());
            var response = await businessLogic.GetUserWeathers(userId);
            if (!response.IsSuccessful)
                return BadRequest(new { message = response.ErrorMessage });

            return Ok(response.Result);
        }


        /// <summary>Use this endpoint to make a partial update of the user widgets.</summary>
        /// <remarks>Instead of using the PATCH conventions, and locate the value using the array location, use the "widgetDescription" to identify the widget to update. As it not possible to add or remove widgets, only the "Replace" operation would be valid.
        /// <para>For an example payload, please see the 'Request body' section.</para>
        /// </remarks>
        [Consumes("application/json-patch+json")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpPatch(Name = "api.userwidget.patch.profilebyid")]
        [SwaggerRequestExample(typeof(Operation[]), typeof(JsonPatchUserWidgetRequestExample))]
        //PATCH :  api/users/widgets
        public async Task<IActionResult> PartialUpdate(
            JsonPatchDocument<UserWidgetForUpdateDto> patchDocument)
        {
            var userId = Guid.Parse(HttpContext.Items["userId"].ToString());
            var response = await this.businessLogic.UpdateUserWidgets(userId, patchDocument);
            if (!response.IsSuccessful)
                return response.RequestResult;
            return NoContent();
        }

        /// <summary>Requests permitted on this URL</summary>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [HttpOptions]
        public IActionResult Options()
        {
            Response.Headers.Add("Allow", "OPTIONS, GET, PATCH");
            return Ok();
        }
    }
}