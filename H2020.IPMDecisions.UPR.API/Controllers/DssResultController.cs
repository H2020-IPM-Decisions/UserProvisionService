using System;
using System.Net.Mime;
using System.Threading.Tasks;
using H2020.IPMDecisions.UPR.API.Filters;
using H2020.IPMDecisions.UPR.BLL;
using H2020.IPMDecisions.UPR.Core.Dtos;
using H2020.IPMDecisions.UPR.Core.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace H2020.IPMDecisions.UPR.API.Controllers
{
    /// <summary>
    /// These end points allows to manage the results of a DSS.
    /// <para>The user will be identified using the UserId on the authentification JWT.</para>
    /// </summary>
    [Obsolete]
    [ApiController]
    [Route("api/dss/{dssId:guid}/result")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [ServiceFilter(typeof(AddUserIdToContextFilter))]
    public class DssResultController : ControllerBase
    {
        private readonly IBusinessLogic businessLogic;
        public DssResultController(IBusinessLogic businessLogic)
        {
            this.businessLogic = businessLogic
                ?? throw new System.ArgumentNullException(nameof(businessLogic));
        }

        /// <summary>
        /// Use this request to get the result the latest result related to a Field Crop Pest DSS
        /// </summary>
        /// <remarks>The user will be identified using the UserId on the authentification JWT.
        /// </remarks>
        [ProducesResponseType(typeof(FieldDssResultDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Produces(MediaTypeNames.Application.Json)]
        [HttpGet(Name = "api.dssresult.get.latest")]
        [HttpHead]
        // GET: api/dss/1/result
        public async Task<IActionResult> GetLatest([FromRoute] Guid dssId)
        {
            var userId = Guid.Parse(HttpContext.Items["userId"].ToString());

            GenericResponse<FieldDssResultDto> response = await businessLogic.GetLatestFieldCropPestDssResult(dssId, userId);

            if (!response.IsSuccessful)
                return BadRequest(new { message = response.ErrorMessage });

            return Ok(response.Result);
        }

        /// <summary>
        /// Use this request to save the result of a DSS
        /// </summary>
        /// <remarks>The user will be identified using the UserId on the authentification JWT.
        /// <para>The DSS must belong to the user</para>
        /// </remarks>
        [Consumes(MediaTypeNames.Application.Json)]
        [ProducesResponseType(typeof(FieldDssResultDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [Produces(MediaTypeNames.Application.Json)]
        [HttpPost(Name = "api.dssresult.post.result")]
        // POST: api/dss/1/result
        public async Task<IActionResult> Post(
            [FromRoute] Guid dssId,
            [FromBody] FieldDssResultForCreationDto dssResultDto)
        {
            var userId = Guid.Parse(HttpContext.Items["userId"].ToString());

            GenericResponse<FieldDssResultDto> response = await businessLogic.CreateFieldCropPestDssResult(dssId, userId, dssResultDto);

            if (!response.IsSuccessful)
                return BadRequest(new { message = response.ErrorMessage });

            return Ok(response.Result);
        }

        // <summary>Requests permitted on this URL</summary>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [HttpOptions]
        public IActionResult Options([FromRoute] Guid dssId)
        {
            Response.Headers.Add("Allow", "OPTIONS, GET, POST");
            return Ok();
        }
    }
}