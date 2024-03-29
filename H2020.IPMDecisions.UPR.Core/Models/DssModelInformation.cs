using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace H2020.IPMDecisions.UPR.Core.Models
{
    // This class matches the schema definition of /api/dss/rest/schema/dss - Model section
    public class DssModelInformation
    {
        public string Id { get; set; }
        public string Name { get; set; }
        [JsonProperty("type_of_decision")]
        public string TypeOfDecision { get; set; }
        [JsonProperty("type_of_output")]
        public string TypeOfOutput { get; set; }
        [JsonProperty("description_URL")]
        public string DescriptionUrl { get; set; }
        public string Description { get; set; }
        public string Version { get; set; }
        // public string Citation { get; set; }
        public string Keywords { get; set; }
        public List<string> Pests { get; set; }
        public List<string> Crops { get; set; }
        public DssModelSchemaExecution Execution { get; set; }
        public DssModelSchemaInput Input { get; set; }
        public DssModelSchemaOutput Output { get; set; }
        [JsonProperty("valid_spatial")]
        public DssModelValidSpatial ValidSpatial { get; set; }
        public IEnumerable<DssModelAuthors> Authors { get; set; }
        [JsonProperty("platform_validated")]
        public bool PlatformValidated { get; set; }
        public string Purpose { get; set; }

        // These properties are not on original schema
        public bool WeatherParametersValidated { get; set; }
        public bool AlreadySavedByUser { get; set; }
        public Guid DssDatabaseId { get; set; }
    }

    public class DssModelValidSpatial
    {
        public IEnumerable<string> Countries { get; set; }
        public string GeoJson { get; set; }
    }

    public class DssModelAuthors
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public string Organization { get; set; }
    }

    public class DssModelSchemaOutput
    {
        [JsonProperty("warning_status_interpretation")]
        public List<DssWarningStatusInterpretation> ListWarningStatusInterpretation { get; set; }
        [JsonProperty("result_parameters")]
        public IEnumerable<OutputResultParameter> ResultParameters { get; set; }
        [JsonProperty("chart_heading")]
        public string ChartHeading { get; set; }
        [JsonProperty("chart_groups")]
        public IEnumerable<OutputChartGroup> ChartGroups { get; set; }
    }

    public class DssWarningStatusInterpretation
    {
        public string Explanation { get; set; }
        [JsonProperty("recommended_action")]
        public string RecommendedAction { get; set; }
    }

    public class OutputChartGroup
    {
        public string Id { get; set; }
        public string Title { get; set; }
        [JsonProperty("result_parameter_ids")]
        public IEnumerable<string> ResultParameterIds { get; set; }
    }

    public class OutputResultParameter
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

    public class DssModelSchemaInput
    {
        [JsonProperty("weather_parameters")]
        public List<WeatherParameters> WeatherParameters { get; set; }
        [JsonProperty("field_observation")]
        public DssSchemaFieldObservation FieldObservation { get; set; }
        [JsonProperty("weather_data_period_start")]
        public IEnumerable<WeatherDataPeriod> WeatherDataPeriodStart { get; set; }
        [JsonProperty("weather_data_period_end")]
        public IEnumerable<WeatherDataPeriod> WeatherDataPeriodEnd { get; set; }
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
        [JsonProperty("is_required")]
        public bool IsRequired { get; set; }
    }

    public class DssModelSchemaExecution
    {
        [JsonProperty("input_schema")]
        public string InputSchema { get; set; }
        public string Type { get; set; }
        public string EndPoint { get; set; }
        [JsonProperty("form_method")]
        public string FormMethod { get; set; }
        [JsonProperty("content_type")]
        public string ContentType { get; set; }
        [JsonProperty("authentication_type")]
        public string AuthenticationType { get; set; }
        [JsonProperty("input_schema_categories")]
        public DssInputSchemaCategories InputSchemaCategories { get; set; }
    }

    public class DssInputSchemaCategories
    {
        public List<string> Hidden { get; set; }
        public List<string> Internal { get; set; }
        public List<string> Triggered { get; set; }
        [JsonProperty("user_init")]
        public List<string> UserInit { get; set; }
        public List<string> System { get; set; }
    }
}