using System;
using System.Linq;
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
    /// <para>The user will be identified using the UserId on the authentication JWT.</para>
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
        /// <remarks>The user will be identified using the UserId on the authentication JWT.
        /// <para>The DSS must belong to the user</para>
        /// </remarks>
        [ProducesResponseType(typeof(AdaptationDashboardDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Produces(MediaTypeNames.Application.Json)]
        [HttpGet("{id:guid}", Name = "api.adaptation.get.byid")]
        // GET: api/dss/1/adaptation
        public async Task<IActionResult> GetAdaptationDataById([FromRoute] Guid id, [FromQuery] int days)
        {
            var userId = Guid.Parse(HttpContext.Items["userId"].ToString());

            var response = await businessLogic.GetAdaptationDataById(id, userId, days);
            if (!response.IsSuccessful)
                return response.RequestResult;

            return Ok(response.Result);
        }

        /// <summary>
        /// Use this request to run a DSS with temp data
        /// </summary>
        /// <remarks>The user will be identified using the UserId on the authentication JWT.
        /// <para>The DSS must belong to the user</para>
        /// </remarks>
        [Consumes(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status202Accepted)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpPost("{id:guid}", Name = "api.adaptation.post.byid")]
        // POST: api/dss/1
        public async Task<IActionResult> Post([FromRoute] Guid id, [FromBody] FieldCropPestDssForUpdateDto fieldCropPestDssForUpdateDto)
        {
            var userId = Guid.Parse(HttpContext.Items["userId"].ToString());

            var response = await businessLogic.AddTaskToRunFieldCropPestDssById(id, userId, fieldCropPestDssForUpdateDto);
            if (!response.IsSuccessful)
                return response.RequestResult;

            var route = "api.adaptation.task.byId";
            var routeValues = new
            {
                dssId = id,
                id = response.Result.Id
            };
            return AcceptedAtRoute(route, routeValues, response.Result);
        }

        /// <summary>
        /// Use this request to run a DSS with temp data
        /// </summary>
        /// <remarks>The user will be identified using the UserId on the authentication JWT.
        /// <para>The DSS must belong to the user</para>
        /// </remarks>
        [Consumes(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpPost("{id:guid}/save", Name = "api.adaptation.postsave.byid")]
        // POST: api/dss/1
        public async Task<IActionResult> Save([FromRoute] Guid id, [FromBody] FieldCropPestDssForAdaptationDto fieldCropPestDssForAdaptationDto)
        {
            var userId = Guid.Parse(HttpContext.Items["userId"].ToString());
            var response = await businessLogic.SaveAdaptationDss(id, userId, fieldCropPestDssForAdaptationDto, HttpContext);
            if (!response.IsSuccessful)
                return response.RequestResult;
            return Ok(response);
        }

        /// <summary>
        /// Use this request to run a DSS with temporal data and compare it against historical data
        /// </summary>
        /// <remarks>The user will be identified using the UserId on the authentication JWT.
        /// <para>The DSS must belong to the user</para>
        /// </remarks>
        [Consumes(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status202Accepted)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpPost("{id:guid}/historicaldata", Name = "api.adaptation.posthistoricaldata.byid")]
        // POST: api/dss/1
        public async Task<IActionResult> PostHistoricalData([FromRoute] Guid id, [FromBody] FieldCropPestDssForHistoricalDataDto fieldCropPestDssForUpdateDto)
        {
            var userId = Guid.Parse(HttpContext.Items["userId"].ToString());
            var response = await businessLogic.PrepareDssToRunFieldCropPestDssHistoricalDataById(id, userId, fieldCropPestDssForUpdateDto);
            if (!response.IsSuccessful)
                return response.RequestResult;

            var route = "api.adaptation.task.byId";
            var routeValues = new
            {
                dssId = id,
                id = response.Result.FirstOrDefault().TaskStatusDto.Id
            };
            return AcceptedAtRoute(route, routeValues, response.Result);
        }

        /// <summary>Use this endpoint to get and specific status of a task.
        /// <para>The DSS must belong to the user</para>
        /// </summary>
        [ProducesResponseType(typeof(DssTaskStatusDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Produces(MediaTypeNames.Application.Json)]
        [HttpGet("{dssId:guid}/task", Name = "api.adaptation.task.byId")]
        // GET: api/1/task?id=5
        public async Task<IActionResult> Get(
            [FromRoute] Guid dssId,
            [FromQuery] string id,
            int days)
        {
            var userId = Guid.Parse(HttpContext.Items["userId"].ToString());
            var response = await this.businessLogic.GetDssResultFromTaskById(dssId, id, userId, days);
            if (!response.IsSuccessful)
                return response.RequestResult;

            return Ok(response.Result);
        }

        // <summary>Requests permitted on this URL</summary>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [HttpOptions]
        public IActionResult Options()
        {
            Response.Headers.Append("Allow", "OPTIONS, GET");
            return Ok();
        }
    }
}