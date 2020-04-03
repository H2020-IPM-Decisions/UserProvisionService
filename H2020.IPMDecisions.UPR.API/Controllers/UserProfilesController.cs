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
                
            return Ok(response.Result);
        }

        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpGet("", Name = "GetUserProfiles")]
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
        [HttpDelete(Name = "DeleteApplicationClient")]
        //DELETE :  api/users/1/profiles
        public async Task<IActionResult> Delete([FromRoute] Guid userId)
        {
            var response = await this.businessLogic.DeleteUserProfileClient(userId);

            if (!response.IsSuccessful)
                return BadRequest(new { message = response.ErrorMessage });
            
            return NoContent();
        }
    }
}