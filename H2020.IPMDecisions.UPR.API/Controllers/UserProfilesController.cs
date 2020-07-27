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

namespace H2020.IPMDecisions.UPR.API.Controllers
{
    [ApiController]
    [Route("api/users/profiles")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [ServiceFilter(typeof(AddUserIdToContextFilter))]
    public class UserProfilesController : ControllerBase
    {
        private readonly IBusinessLogic businessLogic;

        public UserProfilesController(IBusinessLogic businessLogic)
        {
            this.businessLogic = businessLogic 
                ?? throw new ArgumentNullException(nameof(businessLogic));
        }

        [Consumes(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Produces(MediaTypeNames.Application.Json, "application/vnd.h2020ipmdecisions.hateoas+json")]
        [HttpPost("", Name = "api.userprofile.post.profile")]
        // POST: api/users/profiles
        public async Task<IActionResult> Post(
            [FromBody] UserProfileForCreationDto userProfileForCreation,
            [FromHeader(Name = "Accept")] string mediaType)
        {
            var userId = Guid.Parse(HttpContext.Items["userId"].ToString());
            var response = await businessLogic.AddNewUserProfile(userId, userProfileForCreation, mediaType);

            if (!response.IsSuccessful)
                return BadRequest(new { message = response.ErrorMessage });

            return CreatedAtRoute("api.userprofile.get.profilebyid",
                 new { userId },
                 response.Result);
        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Produces(MediaTypeNames.Application.Json, 
        "application/vnd.h2020ipmdecisions.hateoas+json",
        "application/vnd.h2020ipmdecisions.profile.full+json",
        "application/vnd.h2020ipmdecisions.profile.full.hateoas+json",
        "application/vnd.h2020ipmdecisions.profile.friendly.hateoas+json")]
        [HttpGet("", Name = "api.userprofile.get.profilebyid")]
        [HttpHead]
        // GET:  api/users/1/profiles
        public async Task<IActionResult> Get(
            [FromQuery] string fields,
            [FromHeader(Name = "Accept")] string mediaType)
        {
            var userId = Guid.Parse(HttpContext.Items["userId"].ToString());
            var response = await businessLogic.GetUserProfileDto(userId, fields, mediaType);

            if (!response.IsSuccessful)
                return BadRequest(new { message = response.ErrorMessage });
            
            if (response.Result == null)
                return NotFound();
            
            return Ok(response.Result);
        }

        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpDelete(Name = "api.userprofile.delete.profilebyid")]
        //DELETE :  api/users/1/profiles
        public async Task<IActionResult> Delete()
        {
            var userId = Guid.Parse(HttpContext.Items["userId"].ToString());
            var response = await this.businessLogic.DeleteUserProfileClient(userId);

            if (!response.IsSuccessful)
                return BadRequest(new { message = response.ErrorMessage });
            
            return NoContent();
        }

        [Consumes("application/json-patch+json")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpPatch(Name = "api.userprofile.patch.profilebyid")]
        //PATCH :  api/users/1/profiles
        public async Task<IActionResult> PartialUpdate(
            JsonPatchDocument<UserProfileForUpdateDto> patchDocument)
        {
            var userId = Guid.Parse(HttpContext.Items["userId"].ToString());
            var userProfileResponse = await this.businessLogic.GetUserProfile(userId);

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

        [ProducesResponseType(StatusCodes.Status200OK)]
        [HttpOptions]
        public IActionResult Options()
        {
            Response.Headers.Add("Allow", "OPTIONS, GET, PATCH, POST, DELETE");
            return Ok();
        }
    }
}