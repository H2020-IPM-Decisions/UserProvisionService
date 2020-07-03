using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System.Net.Mime;
using H2020.IPMDecisions.UPR.BLL;
using H2020.IPMDecisions.UPR.Core.Dtos;
using H2020.IPMDecisions.UPR.API.Filters;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace H2020.IPMDecisions.IDP.API.Controllers
{
    [ApiController]
    [Route("api/farms/{farmId:guid}/fields")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class FarmFieldsController : ControllerBase
    {
        public FarmFieldsController()
        {
        }
    }
}