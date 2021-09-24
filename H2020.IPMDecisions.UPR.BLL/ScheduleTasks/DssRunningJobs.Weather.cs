using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using H2020.IPMDecisions.UPR.BLL.Helpers;
using H2020.IPMDecisions.UPR.BLL.Providers;
using H2020.IPMDecisions.UPR.Core.Entities;
using H2020.IPMDecisions.UPR.Core.Enums;
using H2020.IPMDecisions.UPR.Core.Models;
using H2020.IPMDecisions.UPR.Data.Core;
using Hangfire;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace H2020.IPMDecisions.UPR.BLL.ScheduleTasks
{
    public partial class DssRunningJobs
    {
        private async Task<GetWeatherDataResult> PrepareWeatherData(FieldCropPestDss dss, DssModelInformation dssInformation, JObject dssInputSchemaAsJson)
        {
            var listOfPreferredWeatherDataSources = new List<WeatherSchemaForHttp>();
            var farm = dss.FieldCropPest.FieldCrop.Field.Farm;
            if (farm.WeatherForecast != null)
            {
                var weatherToCall = this.mapper.Map<WeatherSchemaForHttp>(farm.WeatherForecast);
                listOfPreferredWeatherDataSources.Add(weatherToCall);
            }
            if (farm.WeatherHistorical != null)
            {
                var weatherToCall = this.mapper.Map<WeatherSchemaForHttp>(farm.WeatherHistorical);
                if (dssInformation.Input.WeatherDataPeriodStart != null)
                {
                    var weatherStartDateJsonLocation = dssInformation.Input.WeatherDataPeriodStart.Value.ToString();
                    weatherToCall.WeatherTimeStart = DateTime.Parse(dssInputSchemaAsJson.SelectTokens(weatherStartDateJsonLocation).FirstOrDefault().ToString());
                }
                if (dssInformation.Input.WeatherDataPeriodEnd != null)
                {
                    var weatherEndDateJsonLocation = dssInformation.Input.WeatherDataPeriodEnd.Value.ToString();
                    weatherToCall.WeatherTimeEnd = DateTime.Parse(dssInputSchemaAsJson.SelectTokens(weatherEndDateJsonLocation).FirstOrDefault().ToString());
                }
                listOfPreferredWeatherDataSources.Add(weatherToCall);
            }

            return await GetWeatherData(farm.Location.X.ToString(), farm.Location.Y.ToString(), listOfPreferredWeatherDataSources, dssInformation.Input);
        }

        public async Task<GetWeatherDataResult> GetWeatherData(
            string farmLocationX,
            string farmLocationY,
            List<WeatherSchemaForHttp> listWeatherDataSource,
            DssModelSchemaInput dssWeatherInput)
        {
            var result = new GetWeatherDataResult();
            if (listWeatherDataSource.Count < 1)
            {
                result.ResponseWeather = "No Weather Data Sources or Weather Stations associated to the farm";
                return result;
            }

            //ToDo - Ask what to do when multiple weather data sources associated to a farm. At the moment use only one
            var weatherDataSource = listWeatherDataSource.FirstOrDefault();

            List<string> parameterCodes = dssWeatherInput.WeatherParameters.Select(s => s.ParameterCode.ToString()).ToList();
            weatherDataSource.WeatherDssParameters = string.Join(",", parameterCodes);

            weatherDataSource.Interval = dssWeatherInput.WeatherParameters.FirstOrDefault().Interval;
            var responseWeather = await PrepareWeatherDataCall(farmLocationX, farmLocationY, weatherDataSource);

            result.Continue = false;
            if (!responseWeather.IsSuccessStatusCode)
            {
                result.ResponseWeather = string.Format("Error getting the weather data - {0}", responseWeather.ReasonPhrase.ToString());
                return result;
            }

            var responseWeatherAsText = await responseWeather.Content.ReadAsStringAsync();
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
            var weatherStringParametersUrl = "";
            if (weatherDataSource.IsForecast)
            {
                weatherStringParametersUrl = string.Format("longitude={0}&latitude={1}",
                    farmLocationX, farmLocationY);
            }
            else
            {
                weatherStringParametersUrl = string.Format("interval={0}&timeStart={1}&timeEnd={2}&ignoreErrors=true",
                    weatherDataSource.Interval.ToString(),
                    weatherDataSource.WeatherTimeStart.ToString("yyyy-MM-dd"),
                    weatherDataSource.WeatherTimeEnd.ToString("yyyy-MM-dd"));
            }
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