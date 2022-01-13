using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using H2020.IPMDecisions.UPR.BLL.Helpers;
using H2020.IPMDecisions.UPR.Core.Entities;
using H2020.IPMDecisions.UPR.Core.Models;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;

namespace H2020.IPMDecisions.UPR.BLL.ScheduleTasks
{
    public partial class DssRunningJobs
    {
        private async Task<WeatherDataResult> PrepareWeatherData(FieldCropPestDss dss, DssModelInformation dssInformation, JObject dssInputSchemaAsJson)
        {
            try
            {
                var listOfPreferredWeatherDataSources = new List<WeatherSchemaForHttp>();
                var farm = dss.FieldCropPest.FieldCrop.Field.Farm;
                var currentYear = DssDataHelper.GetCurrentYearForDssDefaultDates(dssInformation, dssInputSchemaAsJson);
                if (farm.WeatherForecast != null)
                {
                    var weatherInformation = await this.internalCommunicationProvider
                           .GetWeatherProviderInformationFromWeatherMicroservice(farm.WeatherForecast.WeatherId);

                    var weatherToCall = this.mapper.Map<WeatherSchemaForHttp>(weatherInformation);
                    weatherToCall.IsForecast = true;
                    AddWeatherDates(dssInformation, dssInputSchemaAsJson, weatherToCall, currentYear);
                    listOfPreferredWeatherDataSources.Add(weatherToCall);
                }
                if (farm.WeatherHistorical != null)
                {
                    var weatherInformation = await this.internalCommunicationProvider
                           .GetWeatherProviderInformationFromWeatherMicroservice(farm.WeatherHistorical.WeatherId);

                    var weatherToCall = this.mapper.Map<WeatherSchemaForHttp>(weatherInformation);
                    AddWeatherDates(dssInformation, dssInputSchemaAsJson, weatherToCall, currentYear);
                    listOfPreferredWeatherDataSources.Add(weatherToCall);
                }

                return await GetWeatherData(farm.Location.X.ToString(), farm.Location.Y.ToString(), listOfPreferredWeatherDataSources, dssInformation.Input);
            }
            catch (Exception ex)
            {
                return new WeatherDataResult()
                {
                    Continue = false,
                    ResponseWeather = ex.Message.ToString()
                };
            }
        }

        private static void AddWeatherDates(DssModelInformation dssInformation, JObject dssInputSchemaAsJson, WeatherSchemaForHttp weatherToCall, int currentYear = -1)
        {
            if (dssInformation.Input.WeatherDataPeriodEnd != null)
            {
                weatherToCall.WeatherTimeEnd = DssDataHelper.ProcessWeatherDataPeriod(dssInformation.Input.WeatherDataPeriodEnd, dssInputSchemaAsJson, currentYear);
            }
            if (dssInformation.Input.WeatherDataPeriodStart != null)
            {
                weatherToCall.WeatherTimeStart = DssDataHelper.ProcessWeatherDataPeriod(dssInformation.Input.WeatherDataPeriodStart, dssInputSchemaAsJson, currentYear);
                if (weatherToCall.WeatherTimeStart > DateTime.Today)
                {
                    throw new InvalidDataException(
                        string.Format("Weather data is not currently available for next season, please try again after {0}",
                        weatherToCall.WeatherTimeStart.ToShortDateString()));
                }
            }
        }

        public async Task<WeatherDataResult> GetWeatherData(
            string farmLocationX,
            string farmLocationY,
            List<WeatherSchemaForHttp> listWeatherDataSource,
            DssModelSchemaInput dssWeatherInput)
        {
            var result = new WeatherDataResult();
            if (listWeatherDataSource.Count < 1)
            {
                result.ResponseWeather = "No Weather Data Sources or Weather Stations associated to the farm";
                return result;
            }

            //ToDo - Ask what to do when multiple weather data sources associated to a farm. At the moment use only one
            var weatherDataSource = listWeatherDataSource.FirstOrDefault();

            PrepareWeatherDssParameters(dssWeatherInput, weatherDataSource);
            weatherDataSource.Interval = dssWeatherInput.WeatherParameters.FirstOrDefault().Interval;

            // Use only debug files for November Demo
            var responseWeatherAsText = GetWeatherDataTestFile(weatherDataSource);
            // var responseWeather = await PrepareWeatherDataCall(farmLocationX, farmLocationY, weatherDataSource);
            // result.Continue = false;
            // if (!responseWeather.IsSuccessStatusCode)
            // {
            //     var responseText = await responseWeather.Content.ReadAsStringAsync();
            //     // Amalgamation service error
            //     Regex regex = new Regex(@".{30}\d{3}.{42}[:]",
            //     RegexOptions.IgnoreCase | RegexOptions.IgnorePatternWhitespace);
            //     if (regex.IsMatch(responseText))
            //     {
            //         responseText = regex.Replace(responseText, "", 1);
            //         result.ResponseWeather = responseText.Trim();
            //         return result;
            //     }
            //     result.ResponseWeather = string.Format("{0} - {1}", responseWeather.ReasonPhrase.ToString(), responseText);
            //     return result;
            // }
            // var responseWeatherAsText = await responseWeather.Content.ReadAsStringAsync();

            if (!DataParseHelper.IsValidJson(responseWeatherAsText))
            {
                result.ResponseWeather = "Weather data received in not in a JSON format.";
                return result;
            };

            if (!await ValidateWeatherDataSchema(responseWeatherAsText))
            {
                result.ResponseWeather = string.Format("Weather data received failed the Weather validation schema. This might be because the weather data source selected do not accept weather parameters required by the DSS: {0}", weatherDataSource.WeatherDssParameters.ToString());
                return result;
            };
            result.Continue = true;
            result.ResponseWeather = responseWeatherAsText;
            return result;
        }

        private string GetWeatherDataTestFile(WeatherSchemaForHttp weatherDataSource)
        {
            try
            {
                var assembly = Assembly.GetExecutingAssembly();
                var fileName = "IPMDecision_FullSeasonWeather";
                if (weatherDataSource.Interval == 86400)
                {
                    fileName = fileName + "_Daily.json";
                }
                else
                {
                    fileName = fileName + "_Hourly.json";

                }
                var resourceName = string.Format("H2020.IPMDecisions.UPR.BLL.Files.{0}", fileName);

                using (Stream stream = assembly.GetManifestResourceStream(resourceName))
                using (StreamReader reader = new StreamReader(stream))
                {
                    return reader.ReadToEnd();
                }
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
            var matching = weatherDataSource
                .WeatherParameters
                .Where(w =>
                    parameterCodes
                    .Any(p => p.ToString() == w.ToString()))
            .ToList();

            if (matching.Count() == 0)
            {
                // Use default Weather Service parameters
                weatherDataSource.WeatherDssParameters = string.Join(",", weatherDataSource.WeatherParameters);
            }
            else
            {
                // Only use the ones needed by the DSS
                weatherDataSource.WeatherDssParameters = string.Join(",", matching);
            }
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
            string farmLocationX,
            string farmLocationY,
            WeatherSchemaForHttp weatherDataSource)
        {
            var weatherStringParametersUrl = string.Format("longitude={0}&latitude={1}&interval={2}&timeStart={3}&timeEnd={4}&ignoreErrors=true",
                farmLocationX,
                farmLocationY,
                weatherDataSource.Interval.ToString(),
                weatherDataSource.WeatherTimeStart.ToString("yyyy-MM-dd"),
                weatherDataSource.WeatherTimeEnd.ToString("yyyy-MM-dd"));

            if (!string.IsNullOrEmpty(weatherDataSource.WeatherDssParameters))
            {
                weatherStringParametersUrl = string.Format("{0}&parameters={1}",
                    weatherStringParametersUrl, weatherDataSource.WeatherDssParameters);
            }
            // if (!string.IsNullOrEmpty(weatherDataSource.StationId))
            // {
            //     weatherUrl = string.Format("{0}&weatherStationId={1}",
            //         weatherUrl, weatherDataSource.StationId.ToString());
            // }
            return await
                    internalCommunicationProvider
                    .GetWeatherUsingAmalgamationService(weatherDataSource.Url, weatherStringParametersUrl);
        }
    }
}