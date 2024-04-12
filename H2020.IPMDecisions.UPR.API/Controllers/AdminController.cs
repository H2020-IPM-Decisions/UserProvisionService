using System;
using System.Collections.Generic;
using System.Net.Mime;
using System.Threading.Tasks;
using H2020.IPMDecisions.UPR.BLL;
using H2020.IPMDecisions.UPR.Core.Dtos;
using H2020.IPMDecisions.UPR.Core.Enums;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace H2020.IPMDecisions.UPR.API.Controllers
{
    /// <summary>
    /// These end points allows administrators to manage default values used by the system.
    /// <para>The user will be identified using the "Admin" claim on the authentication JWT.</para>
    /// </summary>
    [ApiController]
    [Route("api/admin")]
    [Authorize(Roles = "Admin",
        AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class AdminController : ControllerBase
    {
        private readonly IBusinessLogic businessLogic;
        public AdminController(IBusinessLogic businessLogic)
        {
            this.businessLogic = businessLogic
                ?? throw new System.ArgumentNullException(nameof(businessLogic));
        }

        /// <summary>
        /// Use this request to get the different default variables
        /// </summary>
        /// <remarks>The user will be identified using the "Admin" claim on the authentication JWT.
        /// </remarks>
        [ProducesResponseType(typeof(IEnumerable<AdminVariableDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Produces(MediaTypeNames.Application.Json)]
        [HttpGet("values", Name = "api.admin.get.allvalues")]
        [HttpHead]
        // GET: api/admin/values
        public async Task<IActionResult> GetValues()
        {
            var response = await businessLogic.GetAllAdminVariables();
            if (!response.IsSuccessful)
                return response.RequestResult;

            return Ok(response.Result);
        }

        /// <summary>
        /// Use this request to update a default admin variable
        /// </summary>
        /// <remarks>The user will be identified using the "Admin" claim on the authentication JWT.
        /// </remarks>
        [Consumes(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpPut("values/{id:int}", Name = "api.admin.put.valuesbyid")]
        // PUT: api/admin/values/1
        public async Task<IActionResult> PutValues([FromRoute] AdminValuesEnum id, [FromBody] AdminVariableForManipulationDto adminVariableForManipulationDto)
        {
            var response = await businessLogic.UpdateAdminVariableById(id, adminVariableForManipulationDto);
            if (!response.IsSuccessful)
                return BadRequest(new { message = response.ErrorMessage });

            return NoContent();
        }

        /// <summary>
        /// Use this request to get the DSS have been disable from the platform 
        /// </summary>
        /// <remarks>The user will be identified using the "Admin" claim on the authentication JWT.
        /// </remarks>
        [ProducesResponseType(typeof(IEnumerable<DisabledDssDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Produces(MediaTypeNames.Application.Json)]
        [HttpGet("disabled-dss", Name = "api.admin.get.all-disabled-dss")]
        [HttpHead]
        // GET: api/admin/disabled-dss
        public async Task<IActionResult> GetDss()
        {
            var response = await businessLogic.GetAllDisabledDss();
            if (!response.IsSuccessful)
                return response.RequestResult;

            return Ok(response.Result);
        }

        /// <summary>
        /// Use this request to get to remove Disable DSS from the list
        /// </summary>
        /// <remarks>The user will be identified using the "Admin" claim on the authentication JWT.
        /// </remarks>
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Produces(MediaTypeNames.Application.Json)]
        [HttpDelete("disabled-dss", Name = "api.admin.delete.disabled-dss")]
        [HttpHead]
        // DELETE: api/admin/disabled-dss?ids=1&ids=2
        public async Task<IActionResult> DeleteDss([FromQuery] List<Guid> ids)
        {
            var response = await businessLogic.RemoveDisabledDssFromListAsync(ids);
            if (!response.IsSuccessful)
                return BadRequest(new { message = response.ErrorMessage });

            return NoContent();
        }

        /// <summary>
        /// This endpoint allows to add DSS .
        /// </summary>
        /// <remarks>The user will be identified using the "Admin" claim on the authentication JWT.
        /// </remarks>
        [Consumes(MediaTypeNames.Application.Json)]
        [ProducesResponseType(typeof(DisabledDssDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Produces(MediaTypeNames.Application.Json)]
        [HttpPost("disabled-dss", Name = "api.admin.delete.disabled-dss")]
        // POST: api/admin/disabled-dss
        public async Task<IActionResult> Post(
            [FromBody] IEnumerable<DisabledDssForCreationDto> listOfDisabledDssDto)
        {
            var response = await this.businessLogic.AddDisabledDssFromListAsync(listOfDisabledDssDto);

            if (!response.IsSuccessful)
                return response.RequestResult;

            return Ok(response.Result);
        }

        // <summary>Requests permitted on this URL</summary>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [HttpOptions]
        // OPTION: api/admin
        public IActionResult Options()
        {
            Response.Headers.Append("Allow", "DELETE, POST, OPTIONS, GET, PUT");
            return Ok();
        }
    }
}