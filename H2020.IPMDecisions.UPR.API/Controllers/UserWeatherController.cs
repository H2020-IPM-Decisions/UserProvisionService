using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System.Net.Mime;
using H2020.IPMDecisions.UPR.BLL;
using H2020.IPMDecisions.UPR.API.Filters;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.Collections.Generic;
using H2020.IPMDecisions.UPR.Core.Dtos;
using Microsoft.AspNetCore.JsonPatch;
using Swashbuckle.AspNetCore.Filters;
using Microsoft.AspNetCore.JsonPatch.Operations;
using H2020.IPMDecisions.UPR.Core.PatchOperationExamples;

namespace H2020.IPMDecisions.UPR.API.Controllers
{
    /// <summary>
    /// These end points to manage the widgets for a user.
    /// <para>The user will be identified using the UserId on the authentication JWT.</para>
    /// </summary>
    [ApiController]
    [Route("api/users/weather")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [ServiceFilter(typeof(AddUserIdToContextFilter))]
    [ServiceFilter(typeof(UserAccessingOwnDataActionFilter))]
    public class UserWeatherController : ControllerBase
    {
        private readonly IBusinessLogic businessLogic;

        public UserWeatherController(IBusinessLogic businessLogic)
        {
            this.businessLogic = businessLogic
                ?? throw new ArgumentNullException(nameof(businessLogic));
        }

        /// <summary>
        /// Returns the list of weather stations that belong the user.
        /// </summary>
        /// <remarks>The user will be identified using the UserId on the authentication JWT.
        /// </remarks>
        [ProducesResponseType(typeof(IEnumerable<UserWeatherDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        [Produces(MediaTypeNames.Application.Json)]
        [HttpGet("", Name = "api.userweather.get.all")]
        [HttpHead]
        // GET:  api/users/weather
        public async Task<IActionResult> Get()
        {
            var userId = Guid.Parse(HttpContext.Items["userId"].ToString());
            var response = await businessLogic.GetUserWeathers(userId);
            if (!response.IsSuccessful)
                return BadRequest(new { message = response.ErrorMessage });

            return Ok(response.Result);
        }

        /// <summary>
        /// Returns one weather stations that belong the user.
        /// </summary>
        /// <remarks>The user will be identified using the UserId on the authentication JWT.
        /// <para>The User Weather must belong to the user</para>
        /// </remarks>
        [ProducesResponseType(typeof(UserWeatherDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        [Produces(MediaTypeNames.Application.Json)]
        [HttpGet("{id:guid}", Name = "api.userweather.get.weatherid")]
        // GET:  api/users/weather/5
        public async Task<IActionResult> GetById([FromRoute] Guid id)
        {
            var userId = Guid.Parse(HttpContext.Items["userId"].ToString());
            var response = await businessLogic.GetUserWeatherById(id, userId);
            if (!response.IsSuccessful)
                return BadRequest(new { message = response.ErrorMessage });

            return Ok(response.Result);
        }

        /// <summary>
        /// Create a new weather data source to a user.
        /// </summary>
        /// <remarks>The user will be identified using the UserId on the authentication JWT.
        /// </remarks>
        [Consumes(MediaTypeNames.Application.Json)]
        [ProducesResponseType(typeof(IEnumerable<UserWeatherDto>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        [Produces(MediaTypeNames.Application.Json)]
        [HttpPost("", Name = "api.userweather.post.userid")]
        // POST:  api/users/weather
        public async Task<IActionResult> Post([FromBody] UserWeatherForCreationDto userWeatherForCreationDto)
        {
            var userId = Guid.Parse(HttpContext.Items["userId"].ToString());
            var response = await businessLogic.CreateUserWeather(userId, userWeatherForCreationDto);
            if (!response.IsSuccessful)
                return BadRequest(new { message = response.ErrorMessage });

            return CreatedAtRoute(
                "api.userweather.get.weatherid",
                new { userWeatherId = response.Result.Id },
                response.Result);
        }

        /// <summary>
        /// Use this request to delete a user user weather
        /// </summary>
        /// <remarks>The user will be identified using the UserId on the authentication JWT.
        /// <para>The User Weather must belong to the user</para>
        /// </remarks>
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpDelete("{id:guid}", Name = "api.userweather.delete.byid")]
        //DELETE :  api/users/weather/1
        public async Task<IActionResult> Delete([FromRoute] Guid id)
        {
            var userId = Guid.Parse(HttpContext.Items["userId"].ToString());

            var response = await this.businessLogic.RemoveUserWeather(id, userId);
            if (!response.IsSuccessful)
                return BadRequest(new { message = response.ErrorMessage });

            return NoContent();
        }

        /// <summary>
        /// Use this request to update a user weather
        /// </summary>
        /// <remarks>The user will be identified using the UserId on the authentication JWT.
        /// <para>The User Weather must belong to the user</para>
        /// </remarks>
        [Consumes(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpPut("{id:guid}", Name = "api.userweather.put.byid")]
        // PUT: api/dss/1
        public async Task<IActionResult> Put([FromRoute] Guid id, [FromBody] UserWeatherForUpdateDto userWeatherForUpdateDto)
        {
            var userId = Guid.Parse(HttpContext.Items["userId"].ToString());

            var response = await businessLogic.UpdateUserWeatherById(id, userId, userWeatherForUpdateDto);
            if (!response.IsSuccessful)
                return response.RequestResult;

            return NoContent();
        }

        /// <summary>
        /// Add weather source to a farm.
        /// </summary>
        /// <remarks>The user will be identified using the UserId on the authentication JWT.
        /// </remarks>
        [Consumes(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        [Produces(MediaTypeNames.Application.Json)]
        [HttpPost("{id:guid}/farms", Name = "api.userweather.post.addtofarms")]
        // POST:  api/users/weather/5
        public async Task<IActionResult> AddToFarm([FromRoute] Guid id, [FromBody] List<Guid> farmIds)
        {
            var userId = Guid.Parse(HttpContext.Items["userId"].ToString());
            var response = await businessLogic.AddUserWeatherToFarms(id, userId, farmIds);
            if (!response.IsSuccessful)
                return BadRequest(new { message = response.ErrorMessage });

            return NoContent();
        }

        /// <summary>
        /// Remove weather source to a farm.
        /// </summary>
        /// <remarks>The user will be identified using the UserId on the authentication JWT.
        /// </remarks>
        [Consumes(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        [Produces(MediaTypeNames.Application.Json)]
        [HttpDelete("{id:guid}/farms", Name = "api.userweather.post.addtofarms")]
        // DELETE:  api/users/weather/5
        public async Task<IActionResult> RemoveFromFarm([FromRoute] Guid id, [FromBody] List<Guid> farmIds)
        {
            var userId = Guid.Parse(HttpContext.Items["userId"].ToString());
            var response = await businessLogic.RemoveUserWeatherToFarms(id, userId, farmIds);
            if (!response.IsSuccessful)
                return BadRequest(new { message = response.ErrorMessage });

            return NoContent();
        }

        /// <summary>Requests permitted on this URL</summary>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [HttpOptions]
        public IActionResult Options()
        {
            Response.Headers.Add("Allow", "OPTIONS, DELETE, GET, POST");
            return Ok();
        }
    }
}