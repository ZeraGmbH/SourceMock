using System.Text.Json.Serialization;

namespace RefMeterApi.Models;

/// <summary>
/// 
/// </summary>
[JsonConverter(typeof(JsonStringEnumConverter))]
public enum PllChannel
{
    /// <summary>
    /// 
    /// </summary>
    U1 = 0,
    /// <summary>
    /// 
    /// </summary>
    U2 = 1,
    /// <summary>
    /// 
    /// </summary>
    U3 = 2,
    /// <summary>
    /// 
    /// </summary>
    I1 = 3,
    /// <summary>
    /// 
    /// </summary>
    I2 = 4,
    /// <summary>
    /// 
    /// </summary>
    I3 = 5,
}
