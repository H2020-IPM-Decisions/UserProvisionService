using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using H2020.IPMDecisions.UPR.Core.Dtos;
using H2020.IPMDecisions.UPR.Core.Entities;
using H2020.IPMDecisions.UPR.Core.Models;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

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
                if (weathersAsEntities == null && weathersAsEntities.Count() == 0)
                    return GenericResponseBuilder.Success<IEnumerable<UserWeatherDto>>(weathersToReturn);

                Expression<Func<UserFarm, bool>> expression = uw => uw.UserId == userId;
                var farmsFromTheUser = await this.dataService.UserFarms.FindAllAsync(expression);
                var onlyFarms = farmsFromTheUser.Select(uf => uf.Farm);

                foreach (var userWeather in weathersAsEntities)
                {
                    var getWeatherServiceInformation = await this.internalCommunicationProvider
                        .GetWeatherProviderInformationFromWeatherMicroservice(userWeather.WeatherId);
                    var farmsAsDtos = this.mapper.Map<List<FarmDto>>(onlyFarms.Where(f => f.UserWeatherId == userWeather.Id));
                    var weathersToAdd = this.mapper.Map<UserWeatherDto>(userWeather, opt =>
                    {
                        opt.Items["weatherName"] = getWeatherServiceInformation.Name;
                        opt.Items["farms"] = farmsAsDtos;
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

        public async Task<GenericResponse<UserWeatherDto>> GetUserWeatherById(Guid id, Guid userId)
        {
            try
            {
                Expression<Func<UserWeather, bool>> expression = uw => uw.UserId == userId && uw.Id == id;
                var weatherToReturn = await this.dataService.UserWeathers.FindByConditionAsync(expression);
                if (weatherToReturn == null) return GenericResponseBuilder.NotFound<UserWeatherDto>();

                Expression<Func<Farm, bool>> expressionFarms = f => f.UserWeatherId == id;
                var farmsFromTheUser = await this.dataService.Farms.FindAllByConditionAsync(expressionFarms);
                var farmsAsDtos = this.mapper.Map<List<FarmDto>>(farmsFromTheUser);

                var getWeatherServiceInformation = await this.internalCommunicationProvider
                    .GetWeatherProviderInformationFromWeatherMicroservice(weatherToReturn.WeatherId);
                var dataToReturn = this.mapper.Map<UserWeatherDto>(weatherToReturn, opt =>
                {
                    opt.Items["weatherName"] = getWeatherServiceInformation.Name;
                    opt.Items["farms"] = farmsAsDtos;
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

                bool areValidLoginDetails = await CheckCredentialsWeatherService(getWeatherServiceInformation, userWeatherForCreationDto.UserName, userWeatherForCreationDto.Password, userWeatherForCreationDto.WeatherStationId);
                if (!areValidLoginDetails)
                {
                    return GenericResponseBuilder.NoSuccess<UserWeatherDto>(null, this.jsonStringLocalizer["weather.wrong_login_details"].ToString());
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

        private async Task<bool> CheckCredentialsWeatherService(WeatherDataSchema weatherDataInformation, string userName, string password, string weatherStationId)
        {
            var testBodyPrivateWeather = new WeatherAdapterBodyRequest()
            {
                Credentials = new AdapterCredentials()
                {
                    UserName = userName,
                    Password = password,
                },
                WeatherStationId = weatherStationId,
                Interval = weatherDataInformation.Temporal.Intervals.FirstOrDefault().ToString(),
                Parameters = weatherDataInformation.Parameters.Common.FirstOrDefault().ToString(),
                TimeStart = DateTime.Today.AddDays(-3).ToString("yyyy-MM-dd"),
                TimeEnd = DateTime.Today.AddDays(-1).ToString("yyyy-MM-dd"),
            };
            var collection = testBodyPrivateWeather.ToKeyValuePairList();
            var areValidLoginDetails = await this.internalCommunicationProvider
                .ValidateLoginDetailPersonaWeatherStation(weatherDataInformation.EndPoint.ToString(), collection);
            return areValidLoginDetails;
        }

        public async Task<GenericResponse> RemoveUserWeather(Guid id, Guid userId)
        {
            try
            {
                Expression<Func<UserWeather, bool>> expression = uw => uw.UserId == userId && uw.Id == id;
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

        public async Task<GenericResponse> UpdateUserWeatherById(Guid id, Guid userId, UserWeatherForUpdateDto userWeatherForUpdateDto)
        {
            try
            {
                Expression<Func<UserWeather, bool>> expression = uw => uw.UserId == userId && uw.Id == id;
                var weatherToUpdate = await this.dataService.UserWeathers.FindByConditionAsync(expression);
                if (weatherToUpdate == null) return GenericResponseBuilder.NotFound<UserWeatherDto>();

                var getWeatherServiceInformation = await this.internalCommunicationProvider.GetWeatherProviderInformationFromWeatherMicroservice(weatherToUpdate.WeatherId);

                bool areValidLoginDetails = await CheckCredentialsWeatherService(getWeatherServiceInformation, userWeatherForUpdateDto.UserName, userWeatherForUpdateDto.Password, userWeatherForUpdateDto.WeatherStationId);
                if (!areValidLoginDetails)
                {
                    return GenericResponseBuilder.NoSuccess<UserWeatherDto>(null, this.jsonStringLocalizer["weather.wrong_login_details"].ToString());
                }

                if (!areValidLoginDetails)
                {
                    return GenericResponseBuilder.NoSuccess<UserWeatherDto>(null, this.jsonStringLocalizer["weather.wrong_login_details"].ToString());
                }
                var encryptedPassword = _encryption.Encrypt(userWeatherForUpdateDto.Password);

                var dataToInsert = this.mapper.Map(userWeatherForUpdateDto, weatherToUpdate, opt =>
                {
                    opt.Items["encryptedPassword"] = encryptedPassword;
                });

                this.dataService.UserWeathers.Update(dataToInsert);
                await this.dataService.CompleteAsync();
                return GenericResponseBuilder.Success();
            }
            catch (Exception ex)
            {
                logger.LogError(string.Format("Error in BLL - UpdateUserWeatherById. {0}", ex.Message), ex);
                String innerMessage = (ex.InnerException != null) ? ex.InnerException.Message : "";
                return GenericResponseBuilder.NoSuccess($"{ex.Message} InnerException: {innerMessage}");
            }
        }

        public async Task<GenericResponse> RemoveUserWeatherToFarms(Guid id, Guid userId, List<Guid> farmIds)
        {
            try
            {
                return await ManageUserWeatherOnFarm(id, userId, farmIds, true);
            }
            catch (Exception ex)
            {
                logger.LogError(string.Format("Error in BLL - UpdateUserWeatherById. {0}", ex.Message), ex);
                String innerMessage = (ex.InnerException != null) ? ex.InnerException.Message : "";
                return GenericResponseBuilder.NoSuccess($"{ex.Message} InnerException: {innerMessage}");
            }
        }

        public async Task<GenericResponse> AddUserWeatherToFarms(Guid id, Guid userId, List<Guid> farmIds)
        {
            try
            {
                return await ManageUserWeatherOnFarm(id, userId, farmIds);
            }
            catch (Exception ex)
            {
                logger.LogError(string.Format("Error in BLL - UpdateUserWeatherById. {0}", ex.Message), ex);
                String innerMessage = (ex.InnerException != null) ? ex.InnerException.Message : "";
                return GenericResponseBuilder.NoSuccess($"{ex.Message} InnerException: {innerMessage}");
            }
        }

        private async Task<GenericResponse> ManageUserWeatherOnFarm(Guid id, Guid userId, List<Guid> farmIds, bool remove = false)
        {
            Expression<Func<UserFarm, bool>> expression = uw => uw.UserId == userId;
            var farmsFromTheUser = await this.dataService.UserFarms.FindAllAsync(expression);
            if (farmsFromTheUser == null) return GenericResponseBuilder.NotFound();
            foreach (var farmId in farmIds)
            {
                if (farmsFromTheUser.ToList().Any(f => f.FarmId.Equals(farmId) & f.Farm.UserWeatherId.Equals(id)) & remove)
                {
                    var farm = farmsFromTheUser.Select(uf => uf.Farm).Where(f => f.Id.Equals(farmId)).FirstOrDefault();
                    farm.UserWeatherId = null;
                }
                else if (!farmsFromTheUser.ToList().Any(f => f.FarmId.Equals(farmId) & f.Farm.UserWeatherId.Equals(id)) & !remove)
                {
                    var farm = farmsFromTheUser.Select(uf => uf.Farm).Where(f => f.Id.Equals(farmId)).FirstOrDefault();
                    if (farm != null)
                        farm.UserWeatherId = id;
                }
            }
            await this.dataService.CompleteAsync();
            return GenericResponseBuilder.Success();
        }
    }
}