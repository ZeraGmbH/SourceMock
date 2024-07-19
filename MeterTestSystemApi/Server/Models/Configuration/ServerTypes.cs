using System.Text.Json.Serialization;

namespace MeterTestSystemApi.Models.Configuration;

/// <summary>
/// Known MAD server types.
/// </summary>
[JsonConverter(typeof(JsonStringEnumConverter))]
public enum ServerTypes
{
    /// <summary>
    /// 
    /// </summary>
    STM4000 = 0,

    /// <summary>
    /// 
    /// </summary>
    STM6000 = 1,
}