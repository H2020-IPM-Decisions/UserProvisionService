using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System.Net.Mime;
using H2020.IPMDecisions.UPR.BLL;
using H2020.IPMDecisions.UPR.Core.Dtos;
using H2020.IPMDecisions.UPR.Core.Models;
using System.Security.Claims;

namespace H2020.IPMDecisions.IDP.API.Controllers
{
    [ApiController]
    [Route("api/{userId}/profile")]
    [Authorize()]
    public class UserProfileController : ControllerBase
    {

        private readonly IBusinessLogic businessLogic;

        public UserProfileController(IBusinessLogic businessLogic)
        {
            this.businessLogic = businessLogic 
                ?? throw new ArgumentNullException(nameof(businessLogic));
        }


        [Consumes(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpPost("", Name = "CreateUserProfile")]
        // POST: api/{userId}/profile
        public async Task<IActionResult> Post(
            [FromRoute] Guid userId,
            [FromBody] UserProfileForCreationDto userProfileForCreation)
        {
            var claimsIdentity = this.User.Identity as ClaimsIdentity;
            var userIdFromToken = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier)?.Value;
          
            if (!Guid.TryParse(userIdFromToken, out var validatedGuid))
                return BadRequest(new { message = "UserId on token invalid" });
            if (validatedGuid != userId)
                return Unauthorized();

            var response = await businessLogic.AddNewUserProfile(userId, userProfileForCreation);

            if (response.IsSuccessful)
            {
                var responseAsUserProfile = (GenericResponse<UserProfileDto>)response;
                return Ok(responseAsUserProfile.Result);
            }              

            return BadRequest(new { message = response.ErrorMessage });
        }

        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpGet("", Name = "GetUserProfiles")]
        [HttpHead]
        // GET: api/{userId}/Profile
        public Task<IActionResult> Get()
        {
            throw new NotImplementedException();
        }
    }
}