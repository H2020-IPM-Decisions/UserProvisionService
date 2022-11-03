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
        [HttpGet(Name = "api.admin.get.all")]
        [HttpHead]
        // GET: api/admin
        public async Task<IActionResult> Get()
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
        [HttpPut("{id:int}", Name = "api.admin.put.byid")]
        // PUT: api/admin/1
        public async Task<IActionResult> Put([FromRoute] AdminValuesEnum id, [FromBody] AdminVariableForManipulationDto adminVariableForManipulationDto)
        {
            var response = await businessLogic.UpdateAdminVariableById(id, adminVariableForManipulationDto);
            if (!response.IsSuccessful)
                return BadRequest(new { message = response.ErrorMessage });

            return NoContent();
        }

        // <summary>Requests permitted on this URL</summary>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [HttpOptions]
        // OPTION: api/admin
        public IActionResult Options()
        {
            Response.Headers.Add("Allow", "OPTIONS, GET, PUT");
            return Ok();
        }
    }
}