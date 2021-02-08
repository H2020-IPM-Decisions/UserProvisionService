using System.Collections.Generic;
using System.Net.Mime;
using H2020.IPMDecisions.UPR.BLL.Helpers;
using H2020.IPMDecisions.UPR.Core.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace H2020.IPMDecisions.UPR.API.Controllers
{
    /// <summary>
    /// Root endpoint for the microservice.
    /// </summary>
    [Produces(MediaTypeNames.Application.Json)]
    [Route("/api")]
    [ApiController]
    public class RootController : ControllerBase
    {
        /// <summary>
        /// This request gets the main endpoint from the API.
        /// </summary>
        [HttpGet("", Name = "api.root")]
        public IActionResult GetRoot()
        {
            var links = new List<LinkDto>();

            links.Add(
                new LinkDto(UrlCreatorHelper.GenerateResourceLink(Url, "api.root", new { }),
                "self",
                "GET"));

            return Ok(links);
        }
    }
}