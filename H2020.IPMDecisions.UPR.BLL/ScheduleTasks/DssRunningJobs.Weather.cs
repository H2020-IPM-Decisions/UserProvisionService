using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using H2020.IPMDecisions.UPR.BLL.Helpers;
using H2020.IPMDecisions.UPR.Core.Entities;
using H2020.IPMDecisions.UPR.Core.Enums;
using H2020.IPMDecisions.UPR.Core.Models;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace H2020.IPMDecisions.UPR.BLL.ScheduleTasks
{
    public partial class DssRunningJobs
    {
        private async Task<WeatherDataResult> PrepareWeatherData(FieldCropPestDss dss, DssModelInformation dssInformation, JObject dssInputSchemaAsJson)
        {
            try
            {
                var result = new WeatherDataResult();
                var listOfPreferredWeatherDataSources = new List<WeatherSchemaForHttp>();
                var farm = dss.FieldCropPest.FieldCrop.Field.Farm;
                var currentYear = DssDataHelper.GetCurrentYearForDssDefaultDates(dssInformation, dssInputSchemaAsJson);
                if (farm.WeatherForecast != null)
                {
                    var weatherInformation = await this.internalCommunicationProvider
                           .GetWeatherProviderInformationFromWeatherMicroservice(farm.WeatherForecast.WeatherId);

                    var weatherToCall = this.mapper.Map<WeatherSchemaForHttp>(weatherInformation);
                    weatherToCall.IsForecast = true;
                    result = AddWeatherDates(dssInformation, dssInputSchemaAsJson, weatherToCall, currentYear);
                    listOfPreferredWeatherDataSources.Add(weatherToCall);
                }
                if (farm.WeatherHistorical != null)
                {
                    var weatherInformation = await this.internalCommunicationProvider
                           .GetWeatherProviderInformationFromWeatherMicroservice(farm.WeatherHistorical.WeatherId);

                    var weatherToCall = this.mapper.Map<WeatherSchemaForHttp>(weatherInformation);
                    result = AddWeatherDates(dssInformation, dssInputSchemaAsJson, weatherToCall, currentYear);
                    listOfPreferredWeatherDataSources.Add(weatherToCall);
                }
                if (result.Continue == false) return result;

                return await GetWeatherData(farm.Location.X, farm.Location.Y, listOfPreferredWeatherDataSources, dssInformation.Input);
            }
            catch (Exception ex)
            {
                return new WeatherDataResult()
                {
                    Continue = false,
                    ResponseWeatherAsString = ex.Message.ToString(),
                    ErrorType = DssOutputMessageTypeEnum.Error
                };
            }
        }

        private WeatherDataResult AddWeatherDates(DssModelInformation dssInformation, JObject dssInputSchemaAsJson, WeatherSchemaForHttp weatherToCall, int currentYear = -1)
        {
            var result = new WeatherDataResult() { Continue = true };
            if (dssInformation.Input.WeatherDataPeriodEnd != null)
            {
                weatherToCall.WeatherTimeEnd = DssDataHelper.ProcessWeatherDataPeriod(dssInformation.Input.WeatherDataPeriodEnd, dssInputSchemaAsJson, currentYear);
                if (weatherToCall.WeatherTimeEnd < DateTime.Today.AddDays(15))
                {
                    // ToDo - Waiting for requirements
                    // Send a message to the user about DSS not longer running or change parameters 
                    // depending type of date: Fixed or Parameters
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
                if (weatherToCall.WeatherTimeStart > DateTime.Today)
                {
                    if (dssInformation.Input.WeatherDataPeriodStart.FirstOrDefault().DeterminedBy.ToLower() == "fixed_date")
                    {
                        result.ResponseWeatherAsString = this.jsonStringLocalizer["weather.next_season_fixed",
                        weatherToCall.WeatherTimeStart.ToString("dd/MM/yyyy")].ToString();
                    }
                    else
                    {
                        result.ResponseWeatherAsString = this.jsonStringLocalizer["weather.next_season",
                               weatherToCall.WeatherTimeStart.ToString("dd/MM/yyyy")].ToString();
                    }
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
                var responseText = await responseWeather.Content.ReadAsStringAsync();
                // Amalgamation service error
                Regex regex = new Regex(@".{30}\d{3}.{42}[:]",
                RegexOptions.IgnoreCase | RegexOptions.IgnorePatternWhitespace);
                if (regex.IsMatch(responseText))
                {
                    responseText = regex.Replace(responseText, "", 1);
                    result.ResponseWeatherAsString = responseText.Trim();
                    return result;
                }
                result.ResponseWeatherAsString = string.Format("{0} - {1}", responseWeather.ReasonPhrase.ToString(), responseText);
                return result;
            }
            var responseWeatherAsText = await responseWeather.Content.ReadAsStringAsync();

            if (!DataParseHelper.IsValidJson(responseWeatherAsText))
            {
                result.ResponseWeatherAsString = this.jsonStringLocalizer["weather.no_json_format"].ToString();
                return result;
            };

            if (!await ValidateWeatherDataSchema(responseWeatherAsText))
            {
                result.ResponseWeatherAsString = this.jsonStringLocalizer["weather.validation_error", weatherDataSource.WeatherDssParameters.ToString()].ToString();
                return result;
            };
            result.Continue = true;

            // ToDo Wait to Tor-Einar response about Java error on NIBIO/SEGES DSS
            // result.ResponseWeather = JsonConvert.DeserializeObject<WeatherDataResponseSchema>(responseWeatherAsText);
            // result.ResponseWeather.TimeStart = result.ResponseWeather.TimeStart.ToLocalTime();
            // result.ResponseWeather.TimeEnd = result.ResponseWeather.TimeEnd.ToLocalTime();
            // result.ResponseWeatherAsString = JsonConvert.SerializeObject(result.ResponseWeather);

            result.ResponseWeatherAsString = responseWeatherAsText;
            return result;
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
                    .ValidateWeatherdDataSchemaFromDssMicroservice(weatherDataSchema);
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
                farmLocationX.ToString("G", CultureInfo.InvariantCulture),
                farmLocationY.ToString("G", CultureInfo.InvariantCulture),
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
            return await
                internalCommunicationProvider
                .GetWeatherUsingAmalgamationService(weatherStringParametersUrl);

        }
    }
}