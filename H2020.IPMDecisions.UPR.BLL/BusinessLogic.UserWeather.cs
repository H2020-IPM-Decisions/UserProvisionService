using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using H2020.IPMDecisions.UPR.Core.Dtos;
using H2020.IPMDecisions.UPR.Core.Entities;
using H2020.IPMDecisions.UPR.Core.Models;
using Microsoft.Extensions.Logging;

namespace H2020.IPMDecisions.UPR.BLL
{
    public partial class BusinessLogic : IBusinessLogic
    {
        public async Task<GenericResponse<IEnumerable<UserWeatherDto>>> GetUserWeathers(Guid userId)
        {
            try
            {
                var weathersAsEntities = await this.dataService.UserWeathers.FindByUserIdAsync(userId);
                var weathersToReturn = new List<UserWeatherDto>();
                foreach (var userWeather in weathersAsEntities)
                {
                    var getWeatherServiceInformation = await this.internalCommunicationProvider
                        .GetWeatherProviderInformationFromWeatherMicroservice(userWeather.WeatherId);

                    var weathersToAdd = this.mapper.Map<UserWeatherDto>(userWeather, opt =>
                    {
                        opt.Items["weatherName"] = getWeatherServiceInformation.Name;
                    });

                    weathersToReturn.Add(weathersToAdd);
                }

                return GenericResponseBuilder.Success<IEnumerable<UserWeatherDto>>(weathersToReturn);

            }
            catch (Exception ex)
            {
                logger.LogError(string.Format("Error in BLL - GetUserWeather. {0}", ex.Message), ex);
                String innerMessage = (ex.InnerException != null) ? ex.InnerException.Message : "";
                return GenericResponseBuilder.NoSuccess<IEnumerable<UserWeatherDto>>(null, $"{ex.Message} InnerException: {innerMessage}");
            }
        }

        public async Task<GenericResponse<UserWeatherDto>> GetUserWeatherById(Guid userWeatherId, Guid userId)
        {
            try
            {
                Expression<Func<UserWeather, bool>> expression = uw => uw.UserId == userId && uw.Id == userWeatherId;
                var weatherToReturn = await this.dataService.UserWeathers.FindByConditionAsync(expression);
                if (weatherToReturn == null) return GenericResponseBuilder.NotFound<UserWeatherDto>();

                var getWeatherServiceInformation = await this.internalCommunicationProvider
                    .GetWeatherProviderInformationFromWeatherMicroservice(weatherToReturn.WeatherId);
                var dataToReturn = this.mapper.Map<UserWeatherDto>(weatherToReturn, opt =>
                    {
                        opt.Items["weatherName"] = getWeatherServiceInformation.Name;
                    });

                return GenericResponseBuilder.Success(dataToReturn);
            }
            catch (Exception ex)
            {
                logger.LogError(string.Format("Error in BLL - GetUserWeatherById. {0}", ex.Message));
                String innerMessage = (ex.InnerException != null) ? ex.InnerException.Message : "";
                return GenericResponseBuilder.NoSuccess<UserWeatherDto>(null, $"{ex.Message} InnerException: {innerMessage}");
            }
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
                dataToInsert.UserProfile = currentUserProfileExists.Result;

                this.dataService.UserWeathers.Create(dataToInsert);
                await this.dataService.CompleteAsync();

                var dataToReturn = this.mapper.Map<UserWeatherDto>(dataToInsert, opt =>
                    {
                        opt.Items["weatherName"] = getWeatherServiceInformation.Name;
                    });

                return GenericResponseBuilder.Success(dataToReturn);
            }
            catch (Exception ex)
            {
                logger.LogError(string.Format("Error in BLL - CreateUserWeather. {0}", ex.Message), ex);
                String innerMessage = (ex.InnerException != null) ? ex.InnerException.Message : "";
                return GenericResponseBuilder.NoSuccess<UserWeatherDto>(null, $"{ex.Message} InnerException: {innerMessage}");
            }
        }

        public async Task<GenericResponse> RemoveUserWeather(Guid userWeatherId, Guid userId)
        {
            try
            {
                Expression<Func<UserWeather, bool>> expression = uw => uw.UserId == userId && uw.Id == userWeatherId;
                var weatherToDelete = await this.dataService.UserWeathers.FindByConditionAsync(expression);
                if (weatherToDelete == null) return GenericResponseBuilder.Success();

                this.dataService.UserWeathers.Delete(weatherToDelete);
                await this.dataService.CompleteAsync();

                return GenericResponseBuilder.Success();
            }
            catch (Exception ex)
            {
                logger.LogError(string.Format("Error in BLL - RemoveUserWeather. {0}", ex.Message));
                String innerMessage = (ex.InnerException != null) ? ex.InnerException.Message : "";
                return GenericResponseBuilder.NoSuccess($"{ex.Message} InnerException: {innerMessage}");
            }
        }
    }
}