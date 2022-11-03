using System.Runtime.Serialization;

namespace H2020.IPMDecisions.UPR.Core.Enums
{
    // Enums based on WeatherDataSource , authentication_type property
    public enum WeatherAuthenticationTypeEnum
    {
        [EnumMember(Value = "NONE")]
        None = 0,
        [EnumMember(Value = "CREDENTIALS")]
        Credentials = 1,
        [EnumMember(Value = "BEARER_TOKEN")]
        BearerToken = 2
    }
}