using System;
using System.Net.Mime;
using System.Threading.Tasks;
using H2020.IPMDecisions.UPR.API.Filters;
using H2020.IPMDecisions.UPR.BLL;
using H2020.IPMDecisions.UPR.Core.Dtos;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace H2020.IPMDecisions.UPR.API.Controllers
{
    [ApiController]
    [Route("api/internalcall")]
    [Consumes("application/vnd.h2020ipmdecisions.internal+json")]
    [TypeFilter(typeof(RequestHasInternalTokenResourceFilter))]
    [ApiExplorerSettings(IgnoreApi = true)]
    public class InternalCallsController : ControllerBase
    {
        private readonly IBusinessLogic businessLogic;

        public InternalCallsController(
            IBusinessLogic businessLogic)
        {
            this.businessLogic = businessLogic ??
                throw new ArgumentNullException(nameof(businessLogic));
        }

        [Consumes(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpPost("userprofile", Name = "api.internal.post.profile")]
        // POST: api/internalcall/userprofile
        public async Task<IActionResult> Post(
            [FromBody] UserProfileInternalCallDto userProfileDto)
        {
            var response = await businessLogic.InitialUserProfileCreation(userProfileDto);
            if (response.IsSuccessful)
                return Ok();
            return BadRequest(new { message = response.ErrorMessage });
        }

        [Consumes(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpDelete("userprofile/{userId:guid}", Name = "api.internal.delete.profile")]
        // DELETE: api/internalcall/userprofile
        public async Task<IActionResult> Delete(
            [FromRoute] Guid userId)
        {
            var response = await businessLogic.DeleteUserProfile(userId);
            if (response.IsSuccessful)
                return Ok();
            return BadRequest(new { message = response.ErrorMessage });
        }
    }
}