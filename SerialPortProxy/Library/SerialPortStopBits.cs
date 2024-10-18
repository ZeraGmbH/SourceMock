using System.Text.Json.Serialization;

namespace SerialPortProxy;

/// <summary>
/// 
/// </summary>
[JsonConverter(typeof(JsonStringEnumConverter))]
public enum SerialPortStopBits
{
    /// <summary>
    /// 
    /// </summary>
    None = 0,

    /// <summary>
    /// 
    /// </summary>
    One = 1,

    /// <summary>
    /// 
    /// </summary>
    Two = 2,

    /// <summary>
    /// 
    /// </summary>
    OnePointFive = 3,
}
