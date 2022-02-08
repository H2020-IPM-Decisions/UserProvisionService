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
    /// These end points allows to see all the DSS associated to a user.
    /// <para>The user will be identified using the UserId on the authentification JWT.</para>
    /// </summary>
    [ApiController]
    [Route("api/dss")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [ServiceFilter(typeof(AddUserIdToContextFilter))]
    [ServiceFilter(typeof(AddLanguageToContextFilter))]
    public class DssController : ControllerBase
    {
        private readonly IBusinessLogic businessLogic;
        public DssController(IBusinessLogic businessLogic)
        {
            this.businessLogic = businessLogic
                ?? throw new System.ArgumentNullException(nameof(businessLogic));
        }

        /// <summary>
        /// Use this request to get the DSS related to a user
        /// </summary>
        /// <remarks>The user will be identified using the UserId on the authentification JWT.
        /// </remarks>
        [ProducesResponseType(typeof(IEnumerable<FieldDssResultDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Produces(MediaTypeNames.Application.Json)]
        [HttpGet(Name = "api.dss.get.all")]
        [HttpHead]
        // GET: api/dss
        public async Task<IActionResult> Get()
        {
            var userId = Guid.Parse(HttpContext.Items["userId"].ToString());

            var response = await businessLogic.GetAllDssResults(userId);
            if (!response.IsSuccessful)
                return BadRequest(new { message = response.ErrorMessage });

            return Ok(response.Result);
        }

        /// <summary>
        /// Use this request to get a DSS related to a user
        /// </summary>
        /// <remarks>The user will be identified using the UserId on the authentification JWT.
        /// <para>The DSS must belong to the user</para>
        /// </remarks>
        [ProducesResponseType(typeof(FieldDssResultDetailedDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Produces(MediaTypeNames.Application.Json)]
        [HttpGet("{id:guid}", Name = "api.dss.get.byid")]
        // GET: api/dss/1
        public async Task<IActionResult> GetById([FromRoute] Guid id)
        {
            var userId = Guid.Parse(HttpContext.Items["userId"].ToString());

            var response = await businessLogic.GetFieldCropPestDssById(id, userId);
            if (!response.IsSuccessful)
                return response.RequestResult;

            return Ok(response.Result);
        }

        /// <summary>
        /// Use this request to get the DSS parameters setup by the user
        /// </summary>
        /// <remarks>The user will be identified using the UserId on the authentification JWT.
        /// <para>The DSS must belong to the user</para>
        /// </remarks>
        [ProducesResponseType(typeof(FieldDssResultDetailedDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Produces(MediaTypeNames.Application.Json)]
        [HttpGet("{id:guid}/parameters", Name = "api.dss.getparameters.byid")]
        // GET: api/dss/1/parameters
        public async Task<IActionResult> GetParametersById([FromRoute] Guid id)
        {
            var userId = Guid.Parse(HttpContext.Items["userId"].ToString());

            var response = await businessLogic.GetFieldCropPestDssParametersById(id, userId);
            if (!response.IsSuccessful)
                return response.RequestResult;

            return Content(response.Result, MediaTypeNames.Application.Json);
        }

        /// <summary>
        /// Use this request to update a DSS parameter
        /// </summary>
        /// <remarks>The user will be identified using the UserId on the authentification JWT.
        /// <para>The DSS must belong to the user</para>
        /// </remarks>
        [Consumes(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpPut("{id:guid}", Name = "api.dss.put.byid")]
        // PUT: api/dss/1
        public async Task<IActionResult> Put([FromRoute] Guid id, [FromBody] FieldCropPestDssForUpdateDto fieldCropPestDssForUpdateDto)
        {
            var userId = Guid.Parse(HttpContext.Items["userId"].ToString());

            var response = await businessLogic.UpdateFieldCropPestDssById(id, userId, fieldCropPestDssForUpdateDto);
            if (!response.IsSuccessful)
                return response.RequestResult;

            return NoContent();
        }

        /// <summary>Use this endpoint to remove a DSS by ID
        /// <remarks>The user will be identified using the UserId on the authentification JWT.
        /// <para>The DSS must belong to the user</para>
        /// </remarks>
        /// </summary>
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpDelete("{id:guid}", Name = "api.dss.delete.byid")]
        // DELETE: api/dss/1
        public async Task<IActionResult> Delete([FromRoute] Guid id)
        {
            var userId = Guid.Parse(HttpContext.Items["userId"].ToString());
            var response = await this.businessLogic.DeleteDss(id, userId);

            if (!response.IsSuccessful)
                return BadRequest(new { message = response.ErrorMessage });

            return NoContent();
        }

        // <summary>Requests permitted on this URL</summary>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [HttpOptions]
        public IActionResult Options()
        {
            Response.Headers.Add("Allow", "OPTIONS, GET, DELETE, PUT");
            return Ok();
        }
    }
}