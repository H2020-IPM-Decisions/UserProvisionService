using System;
using System.Net.Mime;
using H2020.IPMDecisions.UPR.API.Filters;
using H2020.IPMDecisions.UPR.BLL;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace H2020.IPMDecisions.UPR.API.Controllers
{
    [ApiController]
    [Route("api/farms/{farmId:guid}/dss")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [ServiceFilter(typeof(AddUserIdToContextFilter), Order = 1)]
    [ServiceFilter(typeof(FarmBelongsToUserActionFilter), Order = 2)]
    public class FarmDssController : ControllerBase
    {
        private readonly IBusinessLogic businessLogic;
        public FarmDssController(IBusinessLogic businessLogic)
        {
            this.businessLogic = businessLogic
                ?? throw new System.ArgumentNullException(nameof(businessLogic));
        }

        [Consumes(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [Produces(MediaTypeNames.Application.Json)]
        [HttpPost("", Name = "api.farmdss.post.dss")]
        // POST: api/farms/1/dss
        public IActionResult Post(
            [FromRoute] Guid farmId,
            [FromBody] object todoObject,
            [FromHeader(Name = "Accept")] string mediaType)
        {
            return Ok();
        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [HttpOptions]
        //OPTIONS: api/farms/1/dss
        public IActionResult Options([FromRoute] Guid fieldId)
        {
            Response.Headers.Add("Allow", "OPTIONS, POST");
            return Ok();
        }
    }
}