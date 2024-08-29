using System.Text.Json.Serialization;

namespace RefMeterApi.Models;

/// <summary>
/// 
/// </summary>
[JsonConverter(typeof(JsonStringEnumConverter))]
public enum ReferenceMeterEMOBSessions
{
    /// <summary>
    /// 
    /// </summary>
    Default = 0,

    /// <summary>
    /// 
    /// </summary>
    DC = 1,

    /// <summary>
    /// 
    /// </summary>
    AC = 2,

    /// <summary>
    /// 
    /// </summary>
    DC4Voltages = 4,
}