using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System.Net.Mime;
using H2020.IPMDecisions.UPR.BLL;
using H2020.IPMDecisions.UPR.Core.Dtos;
using H2020.IPMDecisions.UPR.API.Filters;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Swashbuckle.AspNetCore.Filters;
using H2020.IPMDecisions.UPR.Core.PatchOperationExamples;
using Microsoft.AspNetCore.JsonPatch.Operations;

namespace H2020.IPMDecisions.UPR.API.Controllers
{
    /// <summary>
    /// Use these endpoints to the user profile.
    /// <para>The user will be identified using the UserId on the authentication JWT.</para>
    /// </summary>
    [ApiController]
    [Route("api/users/profiles")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [ServiceFilter(typeof(AddUserIdToContextFilter))]
    [ServiceFilter(typeof(UserAccessingOwnDataActionFilter))]
    public class UserProfilesController : ControllerBase
    {
        private readonly IBusinessLogic businessLogic;

        public UserProfilesController(IBusinessLogic businessLogic)
        {
            this.businessLogic = businessLogic
                ?? throw new ArgumentNullException(nameof(businessLogic));
        }

        /// <summary>
        /// Use this request to create a User Profile.
        /// </summary>
        /// <remarks>User profile is created during registration. This endpoint is useful in case a user profile is deleted.
        /// <para>UserId from query is only for administration purposes</para>
        /// </remarks>
        [Consumes(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Produces(MediaTypeNames.Application.Json,
        "application/vnd.h2020ipmdecisions.hateoas+json",
        "application/vnd.h2020ipmdecisions.profile.full+json",
        "application/vnd.h2020ipmdecisions.profile.full.hateoas+json",
        "application/vnd.h2020ipmdecisions.profile.friendly.hateoas+json")]
        [HttpPost("", Name = "api.userprofile.post.profile")]
        // POST: api/users/profiles?userid=1
        public async Task<IActionResult> Post(
            [FromQuery] Guid userId,
            [FromBody] UserProfileForCreationDto userProfileForCreation,
            [FromHeader(Name = "Accept")] string mediaType)
        {
            var isAdmin = bool.Parse(HttpContext.Items["isAdmin"].ToString());
            if (!isAdmin || userId == default(Guid))
                userId = Guid.Parse(HttpContext.Items["userId"].ToString());

            var response = await businessLogic.AddNewUserProfile(userId, userProfileForCreation, mediaType);
            if (!response.IsSuccessful)
                return BadRequest(new { message = response.ErrorMessage });

            return CreatedAtRoute("api.userprofile.get.profilebyid",
                 new { userId },
                 response.Result);
        }

        /// <summary>
        /// Use this request to get the a user profile
        /// </summary>
        /// <remarks> UserId from query is only for administration purposes
        /// </remarks>
        [ProducesResponseType(typeof(UserProfileDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Produces(MediaTypeNames.Application.Json,
        "application/vnd.h2020ipmdecisions.hateoas+json",
        "application/vnd.h2020ipmdecisions.profile.full+json",
        "application/vnd.h2020ipmdecisions.profile.full.hateoas+json",
        "application/vnd.h2020ipmdecisions.profile.friendly.hateoas+json")]
        [HttpGet("", Name = "api.userprofile.get.profilebyid")]
        [HttpHead]
        // GET:  api/users/profiles
        public async Task<IActionResult> Get(
            [FromQuery] Guid userId, string fields,
            [FromHeader(Name = "Accept")] string mediaType)
        {
            var isAdmin = bool.Parse(HttpContext.Items["isAdmin"].ToString());
            if (!isAdmin || userId == default(Guid))
                userId = Guid.Parse(HttpContext.Items["userId"].ToString());

            var response = await businessLogic.GetUserProfileDto(userId, fields, mediaType);
            if (!response.IsSuccessful)
                return BadRequest(new { message = response.ErrorMessage });

            if (response.Result == null)
                return NotFound();

            return Ok(response.Result);
        }

        /// <summary>
        /// Use this request to get the a user profile
        /// </summary>
        /// <remarks> UserId from query is only for administration purposes
        /// </remarks>
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpDelete(Name = "api.userprofile.delete.profilebyid")]
        //DELETE :  api/users/profiles
        public async Task<IActionResult> Delete([FromQuery] Guid userId)
        {
            var isAdmin = bool.Parse(HttpContext.Items["isAdmin"].ToString());
            if (!isAdmin || userId == default(Guid))
                userId = Guid.Parse(HttpContext.Items["userId"].ToString());

            var response = await this.businessLogic.DeleteUserProfile(userId);
            if (!response.IsSuccessful)
                return BadRequest(new { message = response.ErrorMessage });

            return NoContent();
        }

        /// <summary>
        /// Use this request to partial update a userId
        /// </summary>
        /// <remarks> UserId from query is only for administration purposes/ 
        /// </remarks>
        [Consumes("application/json-patch+json")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpPatch(Name = "api.userprofile.patch.profilebyid")]
        [SwaggerRequestExample(typeof(Operation[]), typeof(JsonPatchUserProfileRequestExample))]
        //PATCH :  api/users/1/profiles
        public async Task<IActionResult> PartialUpdate(
            [FromQuery] Guid userId,
            JsonPatchDocument<UserProfileForUpdateDto> patchDocument)
        {
            var isAdmin = bool.Parse(HttpContext.Items["isAdmin"].ToString());
            if (!isAdmin || userId == default(Guid))
                userId = Guid.Parse(HttpContext.Items["userId"].ToString());

            var userProfileResponse = await this.businessLogic.GetUserProfileByUserId(userId);
            if (!userProfileResponse.IsSuccessful)
                return BadRequest(new { message = userProfileResponse.ErrorMessage });

            if (userProfileResponse.Result == null)
            {
                var userProfileDto = new UserProfileForUpdateDto();
                patchDocument.ApplyTo(userProfileDto, ModelState);
                if (!TryValidateModel(userProfileDto))
                    return ValidationProblem(ModelState);

                var userProfileForCreationDto = this.businessLogic.MapToUserProfileForCreation(userProfileDto);
                if (!TryValidateModel(userProfileForCreationDto))
                    return ValidationProblem(ModelState);

                var createClientResponse = await this.businessLogic.AddNewUserProfile(userId, userProfileForCreationDto);

                if (!createClientResponse.IsSuccessful)
                    return BadRequest(new { message = createClientResponse.ErrorMessage });

                return CreatedAtRoute("api.userprofile.get.profilebyid",
                 new { userId },
                 createClientResponse.Result);
            }

            UserProfileForUpdateDto userProfileToPatch = this.businessLogic.MapToUserProfileForUpdateDto(userProfileResponse.Result);
            patchDocument.ApplyTo(userProfileToPatch, ModelState);
            if (!TryValidateModel(userProfileToPatch))
                return ValidationProblem(ModelState);

            var response = await this.businessLogic.UpdateUserProfile(userProfileResponse.Result, userProfileToPatch);

            if (!response.IsSuccessful)
                return BadRequest(new { message = response.ErrorMessage });

            return NoContent();
        }

        /// <summary>Requests permitted on this URL</summary>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [HttpOptions]
        public IActionResult Options()
        {
            Response.Headers.Add("Allow", "OPTIONS, GET, PATCH, POST, DELETE");
            return Ok();
        }
    }
}