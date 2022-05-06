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
    /// These end points allows to see all the DSS associated to a user.
    /// <para>The user will be identified using the UserId on the authentification JWT.</para>
    /// </summary>
    [ApiController]
    [Route("api/adaptation")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [ServiceFilter(typeof(AddUserIdToContextFilter))]
    public class AdaptationController : ControllerBase
    {
        private readonly IBusinessLogic businessLogic;
        public AdaptationController(IBusinessLogic businessLogic)
        {
            this.businessLogic = businessLogic
                ?? throw new System.ArgumentNullException(nameof(businessLogic));
        }

        /// <summary>
        /// Use this request to get the DSS data for creating the DSS Adaptation Dashboard
        /// </summary>
        /// <remarks>The user will be identified using the UserId on the authentification JWT.
        /// <para>The DSS must belong to the user</para>
        /// </remarks>
        [ProducesResponseType(typeof(AdaptationDashboardDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Produces(MediaTypeNames.Application.Json)]
        [HttpGet("{id:guid}", Name = "api.adaptation.get.byid")]
        // GET: api/dss/1/adaptation
        public async Task<IActionResult> GetAdaptationDataById([FromRoute] Guid id)
        {
            var userId = Guid.Parse(HttpContext.Items["userId"].ToString());

            var response = await businessLogic.GetAdaptationDataById(id, userId);
            if (!response.IsSuccessful)
                return response.RequestResult;

            return Ok(response.Result);
        }

        // /// <summary>
        // /// Use this request to update a DSS parameter
        // /// </summary>
        // /// <remarks>The user will be identified using the UserId on the authentification JWT.
        // /// <para>The DSS must belong to the user</para>
        // /// </remarks>
        // [Consumes(MediaTypeNames.Application.Json)]
        // [ProducesResponseType(StatusCodes.Status204NoContent)]
        // [ProducesResponseType(StatusCodes.Status400BadRequest)]
        // [ProducesResponseType(StatusCodes.Status404NotFound)]
        // [HttpPut("{id:guid}", Name = "api.dss.put.byid")]
        // // PUT: api/dss/1
        // public async Task<IActionResult> Put([FromRoute] Guid id, [FromBody] FieldCropPestDssForUpdateDto fieldCropPestDssForUpdateDto)
        // {
        //     var userId = Guid.Parse(HttpContext.Items["userId"].ToString());

        //     var response = await businessLogic.UpdateFieldCropPestDssById(id, userId, fieldCropPestDssForUpdateDto);
        //     if (!response.IsSuccessful)
        //         return response.RequestResult;

        //     return NoContent();
        // }

        // /// <summary>Use this endpoint to get and specific status of a task.
        // /// <para>The DSS must belong to the user</para>
        // /// </summary>
        // [ProducesResponseType(typeof(DssTaskStatusDto), StatusCodes.Status200OK)]
        // [ProducesResponseType(StatusCodes.Status400BadRequest)]
        // [ProducesResponseType(StatusCodes.Status404NotFound)]
        // [Produces(MediaTypeNames.Application.Json)]
        // [HttpGet("{dssId:guid}/task", Name = "api.dss.task.byId")]
        // // GET: api/1/task/?id=5
        // public async Task<IActionResult> Get(
        //     [FromRoute] Guid dssId,
        //     [FromQuery] string id)
        // {
        //     var userId = Guid.Parse(HttpContext.Items["userId"].ToString());
        //     var response = await this.businessLogic.GetTaskStatusById(dssId, id, userId);
        //     if (!response.IsSuccessful)
        //         return response.RequestResult;

        //     return Ok(response.Result);
        // }

        // <summary>Requests permitted on this URL</summary>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [HttpOptions]
        public IActionResult Options()
        {
            Response.Headers.Add("Allow", "OPTIONS, GET");
            return Ok();
        }
    }
}