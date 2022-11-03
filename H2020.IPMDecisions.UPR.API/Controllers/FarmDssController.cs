using System;
using System.Collections.Generic;
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
    /// <para>The user will be identified using the UserId on the authentication JWT.</para>
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
        /// This endpoint allows to create multiple FieldCropPestDss associated to a farm.
        /// </summary>
        /// <remarks>
        /// <para>If no FieldId is provided, a new field will be autogenerated.</para>
        /// <para>The data will be group by crops, so if two or more records have the same crop EPPO code, will be added into the same field.</para>
        /// <para>If same Crop, Pest and DSS combination is sent on payload, the second occurrence will be omitted.</para>
        /// <para>If any of the DSS submitted has Execution Type "ONTHEFLY", the DSS will start just after saved, although the running time of the DSS is unknown.</para>
        /// <para>A 409 response will be returned if a field (using fieldId parameter) already has a different crop or the same Crop, Pest and DSS combination is submitted.</para>
        /// </remarks>
        [Consumes(MediaTypeNames.Application.Json)]
        [ProducesResponseType(typeof(FieldCropPestDssDto), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(FieldCropPestDssDto), StatusCodes.Status202Accepted)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [Produces(MediaTypeNames.Application.Json)]
        [HttpPost("", Name = "api.farmdss.post.dss")]
        // POST: api/farms/1/dss
        public async Task<IActionResult> Post(
            [FromRoute] Guid farmId,
            [FromBody] IEnumerable<FarmDssForCreationDto> farmDssDto,
            [FromHeader(Name = "Accept")] string mediaType)
        {
            var response = await this.businessLogic.AddListOfFarmDss(farmDssDto, HttpContext, mediaType);

            if (!response.IsSuccessful)
                return response.RequestResult;

            return Ok(response.Result);
        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [HttpOptions]
        //OPTIONS: api/farms/1/dss
        public IActionResult Options([FromRoute] Guid fieldId)
        {
            Response.Headers.Add("Allow", "OPTIONS, GET, POST");
            return Ok();
        }
    }
}