using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System.Net.Mime;
using H2020.IPMDecisions.UPR.BLL;
using H2020.IPMDecisions.UPR.Core.Dtos;
using H2020.IPMDecisions.UPR.Core.Models;
using H2020.IPMDecisions.UPR.API.Filters;
using Microsoft.AspNetCore.JsonPatch;

namespace H2020.IPMDecisions.IDP.API.Controllers
{
    [ApiController]
    [Route("api/users/{userId:guid}/profiles")]
    [Authorize()]
    [ServiceFilter(typeof(UserAccessingOwnDataActionFilter))]
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
        [HttpPost("", Name = "CreateUserProfile")]
        // POST: api/users/1/profiles
        public async Task<IActionResult> Post(
            [FromRoute] Guid userId,
            [FromBody] UserProfileForCreationDto userProfileForCreation)
        {
            var response = await businessLogic.AddNewUserProfile(userId, userProfileForCreation);

            if (!response.IsSuccessful)
                return BadRequest(new { message = response.ErrorMessage });

            return CreatedAtRoute("GetUserProfile",
                 new { userId },
                 response.Result);
        }

        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpGet("", Name = "GetUserProfile")]
        [HttpHead]
        // GET:  api/users/1/profiles
        public async Task<IActionResult> Get([FromRoute] Guid userId)
        {
            var response = await businessLogic.GetUserProfile(userId);

            if (!response.IsSuccessful)
                return BadRequest(new { message = response.ErrorMessage });
            
            if (response.Result == null)
                return NotFound();
            
            return Ok(response.Result);
        }

        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [HttpDelete(Name = "DeleteUserProfile")]
        //DELETE :  api/users/1/profiles
        public async Task<IActionResult> Delete([FromRoute] Guid userId)
        {
            var response = await this.businessLogic.DeleteUserProfileClient(userId);

            if (!response.IsSuccessful)
                return BadRequest(new { message = response.ErrorMessage });
            
            return NoContent();
        }

        [Consumes("application/json-patch+json")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpPatch(Name = "PartialUpdateUserProfile")]
        //PATCH :  api/users/1/profiles
        public async Task<IActionResult> PartialUpdate(
            [FromRoute] Guid userId,
            JsonPatchDocument<UserProfileForUpdateDto> patchDocument)
        {
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

                return CreatedAtRoute("GetUserProfile",
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
    }
}