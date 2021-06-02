using System;
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
    [Route("api/eppocode")]
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
        [ProducesResponseType(typeof(EppoCodeDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Produces(MediaTypeNames.Application.Json)]
        [HttpGet("{eppoCodeType}", Name = "api.eppocode.get.bycode")]
        [HttpHead]
        // GET: api/eppocodes/type?name
        public IActionResult GetEppoCode(
            [FromRoute] string eppoCodeType,
            [FromQuery] string eppoCode)
        {
            throw new NotImplementedException();
        }

        /// <summary>Use this endpoint to get and specific EPPO code.
        /// </summary>
        /// <remarks>Type of EEPO code (pest, crop, etc) is needed.
        /// <p>If a EPPO code is not specified the whole list of the type specified will be returned</p></remarks>
        [ProducesResponseType(typeof(List<string>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Produces(MediaTypeNames.Application.Json)]
        [HttpGet("types", Name = "api.eppocode.get.types")]
        [HttpHead]
        // GET: api/eppocodes/types
        public async Task<IActionResult> GetEppoCodeTypes()
        {
            var response = await businessLogic.GetEppoCodeTypes();
            if (!response.IsSuccessful)
                return BadRequest(new { message = response.ErrorMessage });

            return Ok(response.Result);
        }

        /// <summary>Requests permitted on this URL</summary>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [HttpOptions]
        //OPTIONS: api/eppocodes
        public IActionResult Options()
        {
            Response.Headers.Add("Allow", "OPTIONS, GET, PATCH, POST, DELETE");
            return Ok();
        }
    }
}