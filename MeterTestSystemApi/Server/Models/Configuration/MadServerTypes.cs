using System.Text.Json.Serialization;

namespace MeterTestSystemApi.Models.Configuration;

/// <summary>
/// 
/// </summary>
[JsonConverter(typeof(JsonStringEnumConverter))]
public enum MadServerTypes
{
    /// <summary>
    /// 
    /// </summary>
    STM4000 = 1,

    /// <summary>
    /// 
    /// </summary>
    STM6000 = 2,
}