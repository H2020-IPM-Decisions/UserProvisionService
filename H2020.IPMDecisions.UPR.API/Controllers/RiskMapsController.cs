using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System.Net.Mime;
using H2020.IPMDecisions.UPR.BLL;
using System.Collections.Generic;
using H2020.IPMDecisions.UPR.Core.Dtos;

namespace H2020.IPMDecisions.UPR.API.Controllers
{
    /// <summary>
    /// These end points to manage the risk maps data sources.
    /// </summary>
    [ApiController]
    [Route("api/riskmaps")]
    public class RiskMapsController : ControllerBase
    {
        private readonly IBusinessLogic businessLogic;

        public RiskMapsController(IBusinessLogic businessLogic)
        {
            this.businessLogic = businessLogic
                ?? throw new ArgumentNullException(nameof(businessLogic));
        }

        /// <summary>
        /// Returns the list of risk maps available
        /// </summary>
        [ProducesResponseType(typeof(IEnumerable<RiskMapBaseDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        [Produces(MediaTypeNames.Application.Json)]
        [HttpGet("", Name = "api.riskmaps.get.list")]
        [HttpHead]
        // GET:  api/riskmaps
        public async Task<IActionResult> Get()
        {
            var response = await businessLogic.GetRiskMapDataSources();
            if (!response.IsSuccessful)
                return BadRequest(new { message = response.ErrorMessage });

            return Ok(response.Result);
        }

        /// <summary>
        /// Returns the full risk map details
        /// </summary>
        [ProducesResponseType(typeof(RiskMapBaseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        [Produces(MediaTypeNames.Application.Json)]
        [HttpGet("{id:string}", Name = "api.riskmaps.get.byId")]
        // GET:  api/riskmaps/1
        public async Task<IActionResult> GetById([FromRoute] string id)
        {
            var response = await businessLogic.GetRiskMapDetailedInformation(id);
            if (!response.IsSuccessful)
                return BadRequest(new { message = response.ErrorMessage });

            return Ok(response.Result);
        }

        /// <summary>Requests permitted on this URL</summary>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [HttpOptions]
        public IActionResult Options()
        {
            Response.Headers.Add("Allow", "OPTIONS, GET");
            return Ok();
        }
    }
}