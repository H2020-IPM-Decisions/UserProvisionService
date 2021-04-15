using System;
using System.Net.Mime;
using System.Threading.Tasks;
using H2020.IPMDecisions.UPR.API.Filters;
using H2020.IPMDecisions.UPR.BLL;
using H2020.IPMDecisions.UPR.Core.Dtos;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace H2020.IPMDecisions.UPR.API.Controllers
{
    /// <summary>
    /// These end point allows to create a Field Crop Pest Dss in one call.
    /// <para>The user will be identified using the UserId on the authentification JWT.</para>
    /// </summary>
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

        /// <summary>
        /// This endpoint allows to create a FieldCropPestDss associated to a farm
        /// </summary>
        /// <remarks>If the DSS Execution Type is "ONTHEFLY", a 202 response type will be returned as DSS will start just after saved, although the running time of the DSS is unknown.
        /// </remarks>
        [Consumes(MediaTypeNames.Application.Json)]
        [ProducesResponseType(typeof(FarmWithChildrenDto), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(FarmWithChildrenDto), StatusCodes.Status202Accepted)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [Produces(MediaTypeNames.Application.Json)]
        [HttpPost("", Name = "api.farmdss.post.dss")]
        // POST: api/farms/1/dss
        public async Task<IActionResult> Post(
            [FromRoute] Guid farmId,
            [FromBody] FarmDssForCreationDto farmDssDto,
            [FromHeader(Name = "Accept")] string mediaType)
        {
            var response = await this.businessLogic.AddNewFarmDss(farmDssDto, HttpContext, mediaType);

            if (!response.IsSuccessful)
                return response.RequestResult;

            if (response.RequestResult != null)
                return AcceptedAtRoute(
                    "api.dss.get.byid",
                    new { id = response.Result.FieldCropPestDssDto.Id },
                    response.Result);

            return CreatedAtRoute(
                "api.dss.get.byid",
                new { id = response.Result.FieldCropPestDssDto.Id },
                response.Result);
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