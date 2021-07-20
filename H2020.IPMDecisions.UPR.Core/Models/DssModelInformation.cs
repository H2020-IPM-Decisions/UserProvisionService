using System.Collections.Generic;
using Newtonsoft.Json;

namespace H2020.IPMDecisions.UPR.Core.Models
{
    // This class matches the schema definition of /api/dss/rest/schema/dss - Model section
    public class DssModelInformation
    {
        [JsonProperty("type_of_decision")]
        public string TypeOfDecision { get; set; }
        [JsonProperty("type_of_output")]
        public string TypeOfOutput { get; set; }
        [JsonProperty("description_URL")]
        public string DescriptionUrl { get; set; }
        public string Description { get; set; }
        public DssSchemaExecution Execution { get; set; }
        public DssSchemaInput Input { get; set; }
        public DssSchemaOutput Output { get; set; }
    }

    public class DssSchemaOutput
    {
        [JsonProperty("warning_status_interpretation")]
        public string WarningStatusInterpretation { get; set; }
        [JsonProperty("result_parameters")]
        public List<OutputResultParameters> ResultParameters { get; set; }
    }

    public class OutputResultParameters
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        [JsonProperty("chart_info")]
        public OutputChartInfo ChartInfo { get; set; }
    }

    public class OutputChartInfo
    {
        [JsonProperty("default_visible")]
        public bool DefaultVisible { get; set; }
        public string Unit { get; set; }
        [JsonProperty("chart_type")]
        public string ChartType { get; set; }
        public string Color { get; set; }
    }

    public class DssSchemaInput
    {
        [JsonProperty("weather_parameters")]
        public List<WeatherParameters> WeatherParameters { get; set; }
        [JsonProperty("field_observation")]
        public DssSchemaFieldObservation FieldObservation { get; set; }
        [JsonProperty("weather_data_period_start")]
        public WeatherDataPeriod WeatherDataPeriodStart { get; set; }
        [JsonProperty("weather_data_period_end")]
        public WeatherDataPeriod WeatherDataPeriodEnd { get; set; }
    }

    public class DssSchemaFieldObservation
    {
        public List<string> Species { get; set; }
    }

    public class WeatherDataPeriod
    {
        [JsonProperty("determined_by")]
        public string DeterminedBy { get; set; }
        public string Value { get; set; }
    }

    public class WeatherParameters
    {
        [JsonProperty("parameter_code")]
        public int ParameterCode { get; set; }
        public int Interval { get; set; }
    }

    public class DssSchemaExecution
    {
        [JsonProperty("input_schema")]
        public string InputSchema { get; set; }
        public string Type { get; set; }
        public string EndPoint { get; set; }
        [JsonProperty("form_method")]
        public string FormMethod { get; set; }
    }
}