using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using H2020.IPMDecisions.UPR.Core.Dtos;
using H2020.IPMDecisions.UPR.Core.Entities;
using H2020.IPMDecisions.UPR.Core.Models;
using Microsoft.Extensions.Logging;

namespace H2020.IPMDecisions.UPR.BLL
{
    public partial class BusinessLogic : IBusinessLogic
    {
        public Task<GenericResponse<List<UserWeatherDto>>> GetUserWeather(Guid userId)
        {
            throw new NotImplementedException();
        }

        public async Task<GenericResponse<UserWeatherDto>> CreateUserWeather(Guid userId, UserWeatherForCreationDto userWeatherForCreationDto)
        {
            try
            {
                var getWeatherServiceInformation = await this.internalCommunicationProvider.GetWeatherProviderInformationFromWeatherMicroservice(userWeatherForCreationDto.WeatherId);
                if (getWeatherServiceInformation == null) return GenericResponseBuilder.NoSuccess<UserWeatherDto>(null, $"We can't find weather service with Id {userWeatherForCreationDto.WeatherId}");

                // get userprofile
                var currentUserProfileExists = await GetUserProfileByUserId(userId);
                if (currentUserProfileExists.Result == null)
                {
                    return GenericResponseBuilder.NoSuccess<UserWeatherDto>(null, "User do not have an user profile.");
                }

                var encryptedPassword = _encryption.Encrypt(userWeatherForCreationDto.Password);

                var dataToInsert = this.mapper.Map<UserWeather>(userWeatherForCreationDto, opt =>
                    {
                        opt.Items["weatherName"] = getWeatherServiceInformation.Name;
                        opt.Items["encryptedPassword"] = encryptedPassword;
                    });

                return GenericResponseBuilder.Success<UserWeatherDto>(null);
            }
            catch (Exception ex)
            {
                logger.LogError(string.Format("Error in BLL - CreateUserWeather. {0}", ex.Message), ex);
                String innerMessage = (ex.InnerException != null) ? ex.InnerException.Message : "";
                return GenericResponseBuilder.NoSuccess<UserWeatherDto>(null, $"{ex.Message} InnerException: {innerMessage}");
            }
        }
    }
}