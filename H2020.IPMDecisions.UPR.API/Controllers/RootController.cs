using System.Collections.Generic;
using System.Net.Mime;
using H2020.IPMDecisions.UPR.Core.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace H2020.IPMDecisions.UPR.API.Controllers
{
    [Produces(MediaTypeNames.Application.Json)]
    [Route("/api")]
    [ApiController]
    public class RootController : ControllerBase
    {
        [HttpGet("", Name = "GetRoot")]
        public IActionResult GetRoot()
        {
            var links = new List<LinkDto>();

            links.Add(
                new LinkDto(Url.Link("GetRoot", new { }),
                "self",
                "GET"));

            return Ok(links);
        }
    }
}