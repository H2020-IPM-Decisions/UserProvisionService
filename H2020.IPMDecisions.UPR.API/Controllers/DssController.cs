using System;
using System.Collections.Generic;
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
    /// These end points allows to see all the DSS associated to a user.
    /// <para>The user will be identified using the UserId on the authentication JWT.</para>
    /// </summary>
    [ApiController]
    [Route("api/dss")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [ServiceFilter(typeof(AddUserIdToContextFilter))]
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
        /// <remarks>The user will be identified using the UserId on the authentication JWT.
        /// </remarks>
        [ProducesResponseType(typeof(IEnumerable<FieldDssResultDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Produces(MediaTypeNames.Application.Json)]
        [HttpGet(Name = "api.dss.get.all")]
        [HttpHead]
        // GET: api/dss
        public async Task<IActionResult> Get([FromQuery] DssResultListFilterDto filterDto)
        {
            var userId = Guid.Parse(HttpContext.Items["userId"].ToString());

            var response = await businessLogic.GetAllDssResults(userId, filterDto);
            if (!response.IsSuccessful)
                return BadRequest(new { message = response.ErrorMessage });

            return Ok(response.Result);
        }

        /// <summary>
        /// Use this request to get a DSS related to a user
        /// </summary>
        /// <remarks>The user will be identified using the UserId on the authentication JWT.
        /// <para>The DSS must belong to the user</para>
        /// </remarks>
        [ProducesResponseType(typeof(FieldDssResultDetailedDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Produces(MediaTypeNames.Application.Json)]
        [HttpGet("{id:guid}", Name = "api.dss.get.byid")]
        // GET: api/dss/1
        public async Task<IActionResult> GetById([FromRoute] Guid id, [FromQuery] int days)
        {
            var userId = Guid.Parse(HttpContext.Items["userId"].ToString());

            var response = await businessLogic.GetFieldCropPestDssById(id, userId, days);
            if (!response.IsSuccessful)
                return response.RequestResult;

            return Ok(response.Result);
        }

        /// <summary>
        /// Use this request to get the DSS parameters setup by the user
        /// </summary>
        /// <remarks>The user will be identified using the UserId on the authentication JWT.
        /// <para>The DSS must belong to the user</para>
        /// </remarks>
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Produces(MediaTypeNames.Application.Json)]
        [HttpGet("{id:guid}/parameters", Name = "api.dss.getparameters.byid")]
        // GET: api/dss/1/parameters
        public async Task<IActionResult> GetParametersById([FromRoute] Guid id, [FromQueryAttribute] bool? displayInternal)
        {
            var userId = Guid.Parse(HttpContext.Items["userId"].ToString());

            var response = await businessLogic.GetFieldCropPestDssParametersById(id, userId, displayInternal);
            if (!response.IsSuccessful)
                return response.RequestResult;

            return Ok(response.Result);
        }

        /// <summary>
        /// Use this request to download a CSV file with the season data
        /// </summary>
        /// <remarks>The user will be identified using the UserId on the authentication JWT.
        /// <para>The DSS must belong to the user</para>
        /// </remarks>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpGet("{id:guid}/download", Name = "api.dss.download.byid")]
        // GET: api/dss/1/download
        public async Task<IActionResult> GetDataAsCSV([FromRoute] Guid id)
        {
            var userId = Guid.Parse(HttpContext.Items["userId"].ToString());

            var response = await businessLogic.GetFieldCropPestDssDataAsCSVById(id, userId);
            if (!response.IsSuccessful)
                return response.RequestResult;

            return File(response.Result, "text/csv", "data.csv");
        }

        /// <summary>
        /// Use this request to update a DSS parameter
        /// </summary>
        /// <remarks>The user will be identified using the UserId on the authentication JWT.
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
        /// <remarks>The user will be identified using the UserId on the authentication JWT.
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

        /// <summary>
        /// Use this request to get all the external DSS (link type) related to a user
        /// </summary>
        /// <remarks>The user will be identified using the UserId on the authentication JWT.
        /// </remarks>
        [ProducesResponseType(typeof(IEnumerable<LinkDssDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Produces(MediaTypeNames.Application.Json)]
        [HttpGet("links", Name = "api.dsslink.get.all")]
        [HttpHead]
        // GET: api/dss/links
        public async Task<IActionResult> GetLink()
        {
            var userId = Guid.Parse(HttpContext.Items["userId"].ToString());

            var response = await businessLogic.GetAllLinkDss(userId);
            if (!response.IsSuccessful)
                return BadRequest(new { message = response.ErrorMessage });

            return Ok(response.Result);
        }

        /// <summary>
        /// Use this request to get a list off DSS based on a list of crop and available weather parameters available on farm location
        /// </summary>
        /// <remarks>
        /// <para>CropCodes are a list of Crop EPPO codes comma separated: e.g: cropCodes?DAUCS,HORVW </para>
        ///  <para>executionType filter for types of models. Example values are ONTHEFLY and LINK </para>
        /// </remarks>
        [ProducesResponseType(typeof(IEnumerable<DssInformation>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Produces(MediaTypeNames.Application.Json)]
        [HttpGet("filter", Name = "api.dss.get.filter")]
        // GET: api/dss/filter
        public async Task<IActionResult> GetListFromDssService([FromQuery] DssListFilterDto dssListFilterDto)
        {
            var userId = Guid.Parse(HttpContext.Items["userId"].ToString());
            var response = await businessLogic.GetAllAvailableDssOnFarmLocation(dssListFilterDto, userId);
            if (!response.IsSuccessful)
                return BadRequest(new { message = response.ErrorMessage });

            return Ok(response.Result);
        }

        /// <summary>
        /// Use this request to get the DSS default parameters
        /// </summary>
        /// <remarks>The user will be identified using the UserId on the authentication JWT.
        /// <para>The DSS must belong to the user</para>
        /// </remarks>
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Produces(MediaTypeNames.Application.Json)]
        [HttpGet("{id:guid}/defaultparameters", Name = "api.dss.getdefaultparameters.byid")]
        // GET: api/dss/1/defaultparameters
        public async Task<IActionResult> GetDefaultParametersById([FromRoute] Guid id)
        {
            var userId = Guid.Parse(HttpContext.Items["userId"].ToString());

            var response = await businessLogic.GetFieldCropPestDssDefaultParametersById(id, userId);
            if (!response.IsSuccessful)
                return response.RequestResult;

            return Ok(response.Result);
        }


        /// <summary>
        /// Use this request to add External DSS (link ) to an user
        /// </summary>
        /// <remarks>The user will be identified using the UserId on the authentication JWT.
        /// The farm will be autogenerated, or use the default farm for external dss
        /// </remarks>
        [Consumes(MediaTypeNames.Application.Json)]
        [ProducesResponseType(typeof(FieldCropPestDssDto), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(FieldCropPestDssDto), StatusCodes.Status202Accepted)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [Produces(MediaTypeNames.Application.Json)]
        [HttpPost("links", Name = "api.dss.post.linkdss")]
        // POST: api/dss/links
        public async Task<IActionResult> PostLinks(
            [FromBody] IEnumerable<FarmDssForCreationDto> farmDssDto,
            [FromHeader(Name = "Accept")] string mediaType)
        {
            var response = await this.businessLogic.AddListOfLinkDssToFarm(farmDssDto, HttpContext, mediaType);

            if (!response.IsSuccessful)
                return response.RequestResult;

            return Ok(response.Result);
        }
        // <summary>Requests permitted on this URL</summary>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [HttpOptions]
        public IActionResult Options()
        {
            Response.Headers.Append("Allow", "OPTIONS, GET, DELETE, PUT");
            return Ok();
        }
    }
}