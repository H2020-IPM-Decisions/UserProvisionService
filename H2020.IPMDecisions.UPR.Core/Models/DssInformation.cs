using System.Collections.Generic;
using Newtonsoft.Json;

namespace H2020.IPMDecisions.UPR.Core.Models
{
    // This class matches the schema definition of /api/dss/rest/schema/dss
    public class DssInformation
    {
        public DssSchemaExecution Execution { get; set; }
        public DssSchemaInput Input { get; set; }
    }
    public class DssSchemaInput
    {
        [JsonProperty("Weather_Parameters")]
        public List<WeatherParameters> WeatherParameters { get; set; }
        [JsonProperty("Field_Observation")]
        public DssSchemaFieldObservation FieldObservation { get; set; }
        [JsonProperty("Weather_Data_Period_Start")]
        public WeatherDataPeriod WeatherDataPeriodStart { get; set; }
        [JsonProperty("Weather_Data_Period_End")]
        public WeatherDataPeriod WeatherDataPeriodEnd { get; set; }
    }

    public class DssSchemaFieldObservation
    {
        public List<string> Species { get; set; }
    }

    public class WeatherDataPeriod
    {
        [JsonProperty("Determined_By")]
        public string DeterminedBy { get; set; }
        public string Value { get; set; }
    }

    public class WeatherParameters
    {
        [JsonProperty("Parameter_Code")]
        public int ParameterCode { get; set; }
        public int Interval { get; set; }
    }

    public class DssSchemaExecution
    {
        public string Type { get; set; }
        public string EndPoint { get; set; }
        [JsonProperty("Form_Method")]
        public string FormMethod { get; set; }
    }
}