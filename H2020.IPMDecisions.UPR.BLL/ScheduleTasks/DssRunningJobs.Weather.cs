using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Threading.Tasks;
using H2020.IPMDecisions.UPR.BLL.Helpers;
using H2020.IPMDecisions.UPR.Core.Entities;
using H2020.IPMDecisions.UPR.Core.Enums;
using H2020.IPMDecisions.UPR.Core.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace H2020.IPMDecisions.UPR.BLL.ScheduleTasks
{
    public partial class DssRunningJobs
    {
        private async Task<WeatherDataResult> PrepareWeatherData(FieldCropPestDss dss, DssModelInformation dssInformation, JObject dssInputSchemaAsJson, bool isOnTheFlyData = true)
        {
            try
            {
                var result = new WeatherDataResult();
                var listOfPreferredWeatherDataSources = new List<WeatherSchemaForHttp>();
                var farm = dss.FieldCropPest.FieldCrop.Field.Farm;
                var currentYear = DssDataHelper.GetCurrentYearForDssDefaultDates(dssInformation, dssInputSchemaAsJson);
                var currentHost = config["MicroserviceInternalCommunication:WeatherApiUrl"];
                if (farm.WeatherForecast != null)
                {
                    var weatherInformation = await this.internalCommunicationProvider
                           .GetWeatherProviderInformationFromWeatherMicroservice(farm.WeatherForecast.WeatherId);

                    var weatherToCall = this.mapper.Map<WeatherSchemaForHttp>(weatherInformation, opt =>
                    {
                        opt.Items["host"] = currentHost;
                    });
                    weatherToCall.IsForecast = true;
                    result = AddWeatherDates(dssInformation, dssInputSchemaAsJson, weatherToCall, currentYear, isOnTheFlyData);
                    listOfPreferredWeatherDataSources.Add(weatherToCall);
                }
                if (farm.WeatherHistorical != null)
                {
                    var weatherInformation = await this.internalCommunicationProvider
                           .GetWeatherProviderInformationFromWeatherMicroservice(farm.WeatherHistorical.WeatherId);

                    var weatherToCall = this.mapper.Map<WeatherSchemaForHttp>(weatherInformation, opt =>
                    {
                        opt.Items["host"] = currentHost;
                    });
                    result = AddWeatherDates(dssInformation, dssInputSchemaAsJson, weatherToCall, currentYear, isOnTheFlyData);
                    listOfPreferredWeatherDataSources.Add(weatherToCall);
                }
                if (result.Continue == false) return result;

                return await GetWeatherData(Math.Round(farm.Location.X, 4), Math.Round(farm.Location.Y, 4), listOfPreferredWeatherDataSources, dssInformation.Input);
            }
            catch (Exception ex)
            {
                logger.LogError(string.Format("Error PrepareWeatherData DSS. Id: {0}, Parameters {1}. Error: {2}", dss.Id.ToString(), dss.DssParameters, ex));
                return new WeatherDataResult()
                {
                    Continue = false,
                    ResponseWeatherAsString = ex.Message.ToString(),
                    ErrorType = DssOutputMessageTypeEnum.Error
                };
            }
        }

        private WeatherDataResult AddWeatherDates(DssModelInformation dssInformation, JObject dssInputSchemaAsJson, WeatherSchemaForHttp weatherToCall, int currentYear = -1, bool isOnTheFlyData = false)
        {
            var result = new WeatherDataResult() { Continue = true };
            DateTime originalWeatherEndDate = DateTime.Today.AddDays(-1);
            bool addExtraYear = false;
            if (dssInformation.Input.WeatherDataPeriodEnd != null)
            {
                weatherToCall.WeatherTimeEnd = DssDataHelper.ProcessWeatherDataPeriod(dssInformation.Input.WeatherDataPeriodEnd, dssInputSchemaAsJson, currentYear);
                originalWeatherEndDate = weatherToCall.WeatherTimeEnd;
                if (weatherToCall.WeatherTimeEnd.AddDays(15) < DateTime.Today && bool.Parse(config["AppConfiguration:AutoUpdateToNextSeason"]) && !isOnTheFlyData)
                {
                    if (dssInformation.Input.WeatherDataPeriodStart.FirstOrDefault().DeterminedBy.ToLower() != "fixed_date")
                    {
                        weatherToCall.WeatherTimeEnd = weatherToCall.WeatherTimeEnd.AddYears(1);
                        result.UpdateDssParameters = true;
                        addExtraYear = true;
                    }
                }
                // Forecast weather max 8 days
                if (weatherToCall.WeatherTimeEnd > DateTime.Today.AddDays(8))
                {
                    weatherToCall.WeatherTimeEnd = DateTime.Today.AddDays(8);
                }
            }
            if (dssInformation.Input.WeatherDataPeriodStart != null)
            {
                weatherToCall.WeatherTimeStart = DssDataHelper.ProcessWeatherDataPeriod(dssInformation.Input.WeatherDataPeriodStart, dssInputSchemaAsJson, currentYear);
                if (addExtraYear) weatherToCall.WeatherTimeStart = weatherToCall.WeatherTimeStart.AddYears(1);
                if (weatherToCall.WeatherTimeStart > DateTime.Today)
                {
                    if (dssInformation.Input.WeatherDataPeriodStart.FirstOrDefault().DeterminedBy.ToLower() == "fixed_date")
                    {
                        result.ResponseWeatherAsString = this.jsonStringLocalizer["weather.next_season_fixed",
                            weatherToCall.WeatherTimeStart.ToString("dd/MM"), originalWeatherEndDate.ToString("dd/MM")].ToString();
                    }
                    else
                    {
                        result.ResponseWeatherAsString = this.jsonStringLocalizer["weather.next_season",
                            weatherToCall.WeatherTimeStart.ToString("dd/MM/yyyy")].ToString();
                    }
                    result.Continue = false;
                    result.ErrorType = DssOutputMessageTypeEnum.Warning;
                }
                else if (weatherToCall.WeatherTimeStart > weatherToCall.WeatherTimeEnd)
                {
                    result.ResponseWeatherAsString = this.jsonStringLocalizer["weather.start_end_date_problem",
                        weatherToCall.WeatherTimeStart.ToString("dd/MM/yyyy"), originalWeatherEndDate.ToString("dd/MM/yyyy")].ToString();
                    result.Continue = false;
                    result.ErrorType = DssOutputMessageTypeEnum.Warning;
                }
            }
            return result;
        }

        public async Task<WeatherDataResult> GetWeatherData(
            double farmLocationX,
            double farmLocationY,
            List<WeatherSchemaForHttp> listWeatherDataSource,
            DssModelSchemaInput dssWeatherInput)
        {
            var result = new WeatherDataResult();
            if (listWeatherDataSource.Count < 1)
            {
                result.ResponseWeatherAsString = this.jsonStringLocalizer["weather.missing_weather_service"].ToString();
                return result;
            }

            //ToDo - Ask what to do when multiple weather data sources associated to a farm. At the moment use only one
            var weatherDataSource = listWeatherDataSource.FirstOrDefault();

            // Use only debug files for November Demo
            // var responseWeatherAsText = GetWeatherDataTestFile(dssWeatherInput, weatherDataSource);

            PrepareWeatherDssParameters(dssWeatherInput, weatherDataSource);
            weatherDataSource.Interval = dssWeatherInput.WeatherParameters.FirstOrDefault().Interval;
            var responseWeather = await PrepareWeatherDataCall(farmLocationX, farmLocationY, weatherDataSource);
            result.Continue = false;
            if (!responseWeather.IsSuccessStatusCode)
            {
                // Massive fail on the WX server, some parameters must be wrong!
                if ((int)responseWeather.StatusCode == StatusCodes.Status500InternalServerError)
                {
                    logger.LogError(string.Format("Error weather service: ", responseWeather.RequestMessage.RequestUri.ToString()));
                    WeatherInternalError(result);
                    return result;
                }
                if (responseWeather.Content == null)
                {
                    WeatherInternalError(result);
                    result.ReSchedule = true;
                    return result;
                }
                var responseText = await responseWeather.Content.ReadAsStringAsync();
                if (string.IsNullOrEmpty(responseText))
                {
                    WeatherInternalError(result);
                    result.ReSchedule = true;
                    return result;
                }

                var weatherResponseAsObject = JsonConvert.DeserializeObject<IEnumerable<WeatherErrorResult>>(responseText);
                // Error Code returned by Euroweather for first location request
                if (weatherResponseAsObject.Any(wr => wr.ErrorCode == StatusCodes.Status202Accepted))
                {
                    result.ErrorType = DssOutputMessageTypeEnum.Warning;
                    result.ResponseWeatherAsString = string.Join("; ", weatherResponseAsObject.Select(wr => wr.Message)
                        .Where(wr => !wr.Contains("Internal Server Error")));
                }
                // Error Code returned by Euroweather for requests before time series start
                else if (weatherResponseAsObject.Any(wr => wr.ErrorCode == StatusCodes.Status403Forbidden))
                {
                    result.ErrorType = DssOutputMessageTypeEnum.Error;
                    result.ResponseWeatherAsString = string.Join("; ", weatherResponseAsObject.Select(wr => wr.Message)
                        .Where(wr => !wr.Contains("Internal Server Error")));
                }
                else if (weatherResponseAsObject.Any(wr => wr.ErrorCode == StatusCodes.Status500InternalServerError))
                {
                    result.ReSchedule = true;
                    result.ErrorType = DssOutputMessageTypeEnum.Error;
                    result.ResponseWeatherAsString = this.jsonStringLocalizer["weather.service_busy", "10"].ToString();
                }
                else if (weatherResponseAsObject.Any(wr => wr.ErrorCode == StatusCodes.Status404NotFound))
                {
                    result.ErrorType = DssOutputMessageTypeEnum.Error;
                    result.ResponseWeatherAsString = string.Join("; ", weatherResponseAsObject.Select(wr => wr.Message)
                        .Where(wr => !wr.Contains("Internal Server Error")));
                }
                else
                {
                    WeatherInternalError(result);
                }
                return result;
            }

            var responseWeatherAsText = await responseWeather.Content.ReadAsStringAsync();
            if (!DataParseHelper.IsValidJson(responseWeatherAsText))
            {
                result.ResponseWeatherAsString = this.jsonStringLocalizer["weather.no_json_format"].ToString();
                result.ErrorType = DssOutputMessageTypeEnum.Error;
                return result;
            };

            if (!await ValidateWeatherDataSchema(responseWeatherAsText))
            {
                result.ResponseWeatherAsString = this.jsonStringLocalizer["weather.validation_error", weatherDataSource.WeatherDssParameters.ToString()].ToString();
                result.ErrorType = DssOutputMessageTypeEnum.Error;
                return result;
            };

            responseWeatherAsText = ReorderWeatherParameters(weatherDataSource.WeatherDssParameters, responseWeatherAsText);

            result.Continue = true;
            result.ResponseWeatherAsString = responseWeatherAsText;
            return result;
        }

        private void WeatherInternalError(WeatherDataResult result)
        {
            result.ErrorType = DssOutputMessageTypeEnum.Error;
            result.ResponseWeatherAsString = this.jsonStringLocalizer["weather.internal_error"].ToString();
        }

        private static string ReorderWeatherParameters(string weatherParameterFromDss, string responseWeatherAsText)
        {
            try
            {
                var weatherDataAsObject = JsonConvert.DeserializeObject<WeatherDataResponseSchema>(responseWeatherAsText);
                string weatherParametersFromObject = string.Join(",", weatherDataAsObject.WeatherParameters);
                if (weatherParameterFromDss == weatherParametersFromObject) return responseWeatherAsText;

                var dssParametersAsList = weatherParameterFromDss.Split(",").Select(x => Int32.Parse(x)).ToList();
                Dictionary<int, int> newOldIndex = new Dictionary<int, int>();
                for (int i = 0; i < dssParametersAsList.Count; i++)
                {
                    var oldIndex = weatherDataAsObject.WeatherParameters
                        .FindIndex(p => p == dssParametersAsList[i]);
                    if (oldIndex == -1) continue;
                    newOldIndex.Add(i, oldIndex);
                }
                // Reorder here
                // Do not reader if only one parameter??
                // Refactor needed, extract methods
                var newWeatherObject = new WeatherDataResponseSchema();
                newWeatherObject.Interval = weatherDataAsObject.Interval;
                newWeatherObject.TimeStart = weatherDataAsObject.TimeStart;
                newWeatherObject.TimeEnd = weatherDataAsObject.TimeEnd;
                var dssWeatherParametersAsList = weatherParameterFromDss
                    .Split(",")
                    .Select(x => Int32.Parse(x))
                    .ToList();
                newWeatherObject.WeatherParameters = dssWeatherParametersAsList
                    .Where(wx => weatherDataAsObject.WeatherParameters
                        .Any(dss => dss == wx))
                    .ToList();

                newWeatherObject.LocationWeatherDataResult = new List<LocationWeatherDataResult>();
                for (int i = 0; i < weatherDataAsObject.LocationWeatherDataResult.Count; i++)
                {
                    var locationWeatherDataResult = new LocationWeatherDataResult();
                    locationWeatherDataResult.Altitude = weatherDataAsObject.LocationWeatherDataResult[i].Altitude;
                    locationWeatherDataResult.Latitude = weatherDataAsObject.LocationWeatherDataResult[i].Latitude;
                    locationWeatherDataResult.Longitude = weatherDataAsObject.LocationWeatherDataResult[i].Longitude;
                    locationWeatherDataResult.Length = weatherDataAsObject.LocationWeatherDataResult[i].Length;
                    locationWeatherDataResult.Width = weatherDataAsObject.LocationWeatherDataResult[i].Width;

                    List<int?> newQcList = new List<int?>();
                    List<int?> newAmalgamationList = new List<int?>();
                    foreach (var index in newOldIndex)
                    {
                        var oldIndex = index.Value;
                        newQcList.Add(weatherDataAsObject.LocationWeatherDataResult[i].Qc[oldIndex]);
                        newAmalgamationList.Add(weatherDataAsObject.LocationWeatherDataResult[i].Amalgamation[oldIndex]);
                    }

                    List<List<double?>> newDataList = new List<List<double?>>();
                    foreach (var dataItem in weatherDataAsObject.LocationWeatherDataResult[i].Data)
                    {
                        var dataResult = new List<double?>();
                        foreach (var index in newOldIndex)
                        {
                            var oldIndex = index.Value;
                            dataResult.Add(dataItem[oldIndex]);
                        }
                        newDataList.Add(dataResult);
                    }

                    locationWeatherDataResult.Qc = newQcList;
                    locationWeatherDataResult.Amalgamation = newAmalgamationList;
                    locationWeatherDataResult.Data = newDataList;
                    newWeatherObject.LocationWeatherDataResult.Add(locationWeatherDataResult);
                }
                return JsonConvert.SerializeObject(newWeatherObject);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private string GetWeatherDataTestFile(DssModelSchemaInput dssWeatherInput, WeatherSchemaForHttp weatherDataSource)
        {
            try
            {
                var assembly = Assembly.GetExecutingAssembly();
                var fileName = "IPMDecision_FullSeasonWeather";
                var fileNameInterval = "_Hourly.json";
                var weatherInterval = dssWeatherInput.WeatherParameters.FirstOrDefault().Interval;
                if (weatherInterval == 86400)
                    fileNameInterval = "_Daily.json";
                fileName = fileName + fileNameInterval;

                var resourceName = string.Format("H2020.IPMDecisions.UPR.BLL.Files.{0}", fileName);

                var fileAsString = "";
                using (Stream stream = assembly.GetManifestResourceStream(resourceName))
                using (StreamReader reader = new StreamReader(stream))
                    fileAsString = reader.ReadToEnd();

                var weatherData = JsonConvert.DeserializeObject<WeatherDataResponseSchema>(fileAsString);
                var intervalStartDates = (weatherDataSource.WeatherTimeStart - weatherData.TimeStart);
                var intervalStartEndDates = (weatherDataSource.WeatherTimeEnd - weatherDataSource.WeatherTimeStart);

                var startIndex = 0;
                var lengthBetweenIndex = 0;
                if (weatherInterval == 86400)
                {
                    if (!(weatherDataSource.WeatherTimeStart < weatherData.TimeStart))
                        startIndex = (int)intervalStartDates.TotalDays - 1;
                    lengthBetweenIndex = (int)intervalStartEndDates.TotalDays + 1;
                }
                else
                {
                    if (!(weatherDataSource.WeatherTimeStart < weatherData.TimeStart))
                        startIndex = (int)intervalStartDates.TotalHours - 24;
                    lengthBetweenIndex = (int)intervalStartEndDates.TotalHours + 24;
                }
                var weatherDataResult = weatherData.LocationWeatherDataResult.FirstOrDefault();

                weatherData.LocationWeatherDataResult.FirstOrDefault().Data =
                    weatherDataResult.Data.Skip(startIndex).Take(lengthBetweenIndex).ToList();

                weatherData.LocationWeatherDataResult.FirstOrDefault().Length = weatherDataResult.Data.Count();
                weatherData.TimeStart = weatherDataSource.WeatherTimeStart.ToUniversalTime().AddDays(-1);
                // End date can not be more than the current end date of the file
                if (weatherDataSource.WeatherTimeEnd > weatherData.TimeEnd)
                {
                    weatherDataSource.WeatherTimeEnd = weatherData.TimeEnd;
                }
                weatherData.TimeEnd = weatherDataSource.WeatherTimeEnd.ToUniversalTime();

                // Change WeatherParameters from 1001 to 1002 as we need to use this parameter
                var weatherReplaced = dssWeatherInput.WeatherParameters.Select(p => p.ParameterCode.ToString().Replace("1001", "1002"));

                var listIndexToRemove = new List<int>();
                foreach (var (item, index) in weatherData.WeatherParameters.Select((value, i) => (value, i)))
                {
                    if (!weatherReplaced.Any(w => w.ToString() == item.ToString()))
                    {
                        listIndexToRemove.Add(index);
                    }
                }

                List<int> sortedListIndexToRemove = listIndexToRemove.OrderByDescending(i => i).ToList();
                foreach (var index in sortedListIndexToRemove)
                {
                    weatherData.WeatherParameters.RemoveAt(index);
                    weatherData.LocationWeatherDataResult.FirstOrDefault().Amalgamation.RemoveAt(index);
                    weatherData.LocationWeatherDataResult.FirstOrDefault().Qc.RemoveAt(index);
                    foreach (var dataItem in weatherData.LocationWeatherDataResult.FirstOrDefault().Data)
                    {
                        dataItem.RemoveAt(index);
                    }
                }
                weatherData.LocationWeatherDataResult.FirstOrDefault().Width = weatherData.WeatherParameters.Count();
                return JsonConvert.SerializeObject(weatherData);
            }
            catch (Exception ex)
            {
                ex.Message.ToString();
                throw;
            }
        }

        private static void PrepareWeatherDssParameters(DssModelSchemaInput dssWeatherInput, WeatherSchemaForHttp weatherDataSource)
        {
            List<string> parameterCodes = dssWeatherInput.WeatherParameters.Select(s => s.ParameterCode.ToString()).ToList();
            weatherDataSource.WeatherDssParameters = string.Join(",", parameterCodes);
        }

        public async Task<bool> ValidateWeatherDataSchema(string weatherDataSchema)
        {
            try
            {
                return await
                    internalCommunicationProvider
                    .ValidateWeatherDataSchemaFromDssMicroservice(weatherDataSchema);
            }
            catch (Exception ex)
            {
                logger.LogError(string.Format("Error Validating Weather Data Schema. {0}", ex.Message));
                return false;
            }
        }

        public async Task<HttpResponseMessage> PrepareWeatherDataCall(
            double farmLocationX,
            double farmLocationY,
            WeatherSchemaForHttp weatherDataSource,
            bool useProxy = false)
        {
            var weatherStringParametersUrl = string.Format("longitude={0}&latitude={1}&interval={2}&ignoreErrors=true",
                Math.Round(farmLocationX, 4).ToString("G", CultureInfo.InvariantCulture),
                Math.Round(farmLocationY, 4).ToString("G", CultureInfo.InvariantCulture),
                weatherDataSource.Interval.ToString());

            // ToDo this is for testing purposes as WX data needed from 2021;
            var weatherFormat = "yyyy-MM-dd";
            if (weatherDataSource.WeatherId.ToLower() == "no.nibio.lmt")
            {
                weatherDataSource.StationId = "41";
                useProxy = true;

                weatherDataSource.WeatherTimeStart = new DateTime(2021, 4, 1);
                var startWeatherDate = weatherDataSource.WeatherTimeStart.ToString(weatherFormat, CultureInfo.InvariantCulture);
                startWeatherDate = string.Format("{0}T00:00:00%2B02:00", startWeatherDate);
                var endWeatherDate = weatherDataSource.WeatherTimeEnd.ToString(weatherFormat, CultureInfo.InvariantCulture);
                endWeatherDate = string.Format("{0}T00:00:00%2B02:00", endWeatherDate);

                weatherStringParametersUrl = string.Format("{0}&timeStart={1}&timeEnd={2}",
                    weatherStringParametersUrl,
                    startWeatherDate,
                    endWeatherDate);
            }
            else
            {
                weatherStringParametersUrl = string.Format("{0}&timeStart={1}&timeEnd={2}",
                   weatherStringParametersUrl,
                   weatherDataSource.WeatherTimeStart.ToString(weatherFormat, CultureInfo.InvariantCulture),
                    weatherDataSource.WeatherTimeEnd.ToString(weatherFormat, CultureInfo.InvariantCulture));
            }

            if (!string.IsNullOrEmpty(weatherDataSource.WeatherDssParameters))
            {
                weatherStringParametersUrl = string.Format("{0}&parameters={1}",
                    weatherStringParametersUrl, weatherDataSource.WeatherDssParameters);
            }
            if (!string.IsNullOrEmpty(weatherDataSource.StationId))
            {
                weatherStringParametersUrl = string.Format("{0}&weatherStationId={1}",
                    weatherStringParametersUrl, weatherDataSource.StationId.ToString());
            }
            if (useProxy)
            {
                return await
                    internalCommunicationProvider
                    .GetWeatherUsingAmalgamationProxyService(weatherDataSource.Url, weatherStringParametersUrl);
            }
            // #if DEBUG
            //             bool useOwnService = true;
            //             if (useOwnService)
            //             {
            //                 return await
            //                     internalCommunicationProvider
            //                     .GetWeatherUsingOwnService(weatherDataSource.Url, weatherStringParametersUrl);

            //             }
            // #endif
            return await
                internalCommunicationProvider
                .GetWeatherUsingAmalgamationService(weatherStringParametersUrl);

        }
    }
}