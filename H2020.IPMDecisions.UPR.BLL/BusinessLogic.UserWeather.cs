using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using H2020.IPMDecisions.UPR.Core.Dtos;
using H2020.IPMDecisions.UPR.Core.Models;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.Extensions.Logging;

namespace H2020.IPMDecisions.UPR.BLL
{
    public partial class BusinessLogic : IBusinessLogic
    {
        public Task<GenericResponse<List<UserWeatherDto>>> GetUserWeather(Guid userId)
        {
            throw new NotImplementedException();
        }

        public Task<GenericResponse<UserWeatherDto>> CreateUserWeather(Guid userId, UserWeatherForCreationDto userWeatherForCreationDto)
        {
            throw new NotImplementedException();
        }
    }
}