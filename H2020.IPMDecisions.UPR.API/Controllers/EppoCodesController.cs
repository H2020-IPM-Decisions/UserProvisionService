using System.Collections.Generic;
using System.Net.Mime;
using System.Threading.Tasks;
using H2020.IPMDecisions.UPR.BLL;
using H2020.IPMDecisions.UPR.Core.Dtos;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace H2020.IPMDecisions.UPR.API.Controllers
{
    /// <summary>
    /// These end points allows getting EPPO codes information.
    /// <para>These endpoints are protected by JWT.</para>
    /// </summary>
    [ApiController]
    [Route("api/eppocodes")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class EppoCodesController : ControllerBase
    {
        private readonly IBusinessLogic businessLogic;
        public EppoCodesController(IBusinessLogic businessLogic)
        {
            this.businessLogic = businessLogic
                ?? throw new System.ArgumentNullException(nameof(businessLogic));
        }


        /// <summary>Use this endpoint to get all the EPPO codes on the database, group by type.
        /// </summary>
        [ProducesResponseType(typeof(EppoCodeTypeDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Produces(MediaTypeNames.Application.Json)]
        [HttpGet("", Name = "api.eppocode.get.all")]
        [HttpHead]
        // GET: api/eppocodes
        public async Task<IActionResult> Get()
        {
            var response = await businessLogic.GetAllEppoCodes();
            if (!response.IsSuccessful)
                return BadRequest(new { message = response.ErrorMessage });

            return Ok(response.Result);
        }

        /// <summary>Use this endpoint to get and specific EPPO code.
        /// </summary>
        /// <remarks>Type of EPPO code (pest, crop, etc) is needed.
        /// <p>If a EPPO code is not specified the whole list of the type specified will be returned</p></remarks>
        [ProducesResponseType(typeof(EppoCodeTypeDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Produces(MediaTypeNames.Application.Json)]
        [HttpGet("{eppoCodeType}", Name = "api.eppocode.get.bycode")]
        // GET: api/eppocodes/type?name
        public async Task<IActionResult> GetEppoCode(
            [FromRoute] string eppoCodeType,
            [FromQuery] string eppoCode, string executionType)
        {
            var response = await businessLogic.GetEppoCode(eppoCodeType, eppoCode, executionType);
            if (!response.IsSuccessful)
                return response.RequestResult;

            return Ok(response.Result);
        }

        /// <summary>Use this endpoint to get and specific EPPO code.
        /// </summary>
        /// <remarks>Type of EEPO code (pest, crop, etc) is needed.
        /// <p>If a EPPO code is not specified the whole list of the type specified will be returned</p></remarks>
        [ProducesResponseType(typeof(List<string>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Produces(MediaTypeNames.Application.Json)]
        [HttpGet("types", Name = "api.eppocode.get.types")]
        // GET: api/eppocodes/types
        public async Task<IActionResult> GetEppoCodeTypes()
        {
            var response = await businessLogic.GetEppoCodeTypes();
            if (!response.IsSuccessful)
                return BadRequest(new { message = response.ErrorMessage });

            return Ok(response.Result);
        }

        /// <summary>End point to add new EPPO codes into the database
        /// </summary>
        /// <remarks><p>Endpoint expects full list of EPP codes as JSON string</p>
        /// <p>End point accessible only by administrators</p>
        /// </remarks>
        [ProducesResponseType(typeof(EppoCodeTypeDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Produces(MediaTypeNames.Application.Json)]
        [Consumes(MediaTypeNames.Application.Json)]
        [Authorize(Roles = "Admin", AuthenticationSchemes =
            JwtBearerDefaults.AuthenticationScheme)]
        [HttpPost("", Name = "api.eppocode.post.eppocode")]
        // POST: api/eppocodes
        public async Task<IActionResult> Post(
            [FromBody] EppoCodeForCreationDto eppoCodeForCreationDto)
        {
            var response = await businessLogic.CreateEppoCodeType(eppoCodeForCreationDto);
            if (!response.IsSuccessful)
                return BadRequest(new { message = response.ErrorMessage });

            return CreatedAtRoute(
                "api.eppocode.get.bycode",
                new { eppoCodeType = response.Result.EppoCodeType },
                response.Result);
        }

        /// <summary>End point to update existing EPPO codes
        /// </summary>
        /// <remarks><p>Endpoint expects full list of EPP codes as JSON string</p>
        /// <p>End point accessible only by administrators</p>
        /// </remarks>
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Consumes(MediaTypeNames.Application.Json)]
        [Authorize(Roles = "Admin", AuthenticationSchemes =
            JwtBearerDefaults.AuthenticationScheme)]
        [HttpPut("{eppoCodeType}", Name = "api.eppocode.put.eppocode")]
        // PUT: api/eppocodes/1
        public async Task<IActionResult> Put(
            [FromRoute] string eppoCodeType,
            [FromBody] EppoCodeForUpdateDto eppoCodeForUpdateDto)
        {
            var response = await businessLogic.UpdateEppoCodeType(eppoCodeType, eppoCodeForUpdateDto);
            if (!response.IsSuccessful)
                return response.RequestResult;

            return NoContent();
        }

        /// <summary>Requests permitted on this URL</summary>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [HttpOptions]
        //OPTIONS: api/eppocodes
        public IActionResult Options()
        {
            Response.Headers.Add("Allow", "OPTIONS, GET, HEAD, POST, PUT");
            return Ok();
        }
    }
}