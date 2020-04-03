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
        // GET:  api/users/1/profiles
        public Task<IActionResult> Get([FromRoute] Guid userId)
        {
            throw new NotImplementedException();
        }
    }
}