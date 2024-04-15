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
    /// These end points allows to compare DSSs associated to a user.
    /// <para>The user will be identified using the UserId on the authentication JWT.</para>
    /// </summary>
    [ApiController]
    [Route("api/dsscomparison")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [ServiceFilter(typeof(AddUserIdToContextFilter))]
    public class DssComparisonController : ControllerBase
    {
        private readonly IBusinessLogic businessLogic;
        public DssComparisonController(IBusinessLogic businessLogic)
        {
            this.businessLogic = businessLogic
                ?? throw new System.ArgumentNullException(nameof(businessLogic));
        }

        /// <summary>
        /// Use this request to compare DSS related to a user
        /// </summary>
        /// <remarks>The user will be identified using the UserId on the authentication JWT.
        /// <para>Due to the GUID complexity, the query parameters should be like the following:
        /// api/upr/dsscomparison?dssids=firstDssId&amp;dssids=secondDssId&amp;dssids=otherDssId</para>
        /// <para> It is a limit of 5 DSS ids on the list</para> 
        /// </remarks>
        [ProducesResponseType(typeof(IEnumerable<FieldDssResultDetailedDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Produces(MediaTypeNames.Application.Json)]
        [HttpGet(Name = "api.dsscomparison.get.ids")]
        [HttpHead]
        // GET: api/dsscomparison?dssids=id1&dssids=id2&days=3
        public async Task<IActionResult> Get([FromQuery] ComparisonDashboardDto comparasionData)
        {
            var userId = Guid.Parse(HttpContext.Items["userId"].ToString());

            var response = await businessLogic.CompareDssByIds(comparasionData.DssIds, userId, comparasionData.Days);
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