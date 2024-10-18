using System.Text.Json.Serialization;

namespace SerialPortProxy;

/// <summary>
/// 
/// </summary>
[JsonConverter(typeof(JsonStringEnumConverter))]
public enum SerialPortParity
{
    /// <summary>
    /// 
    /// </summary>
    None = 0,

    /// <summary>
    /// 
    /// </summary>
    Odd = 1,

    /// <summary>
    /// 
    /// </summary>
    Even = 2,

    /// <summary>
    /// 
    /// </summary>
    Mark = 3,

    /// <summary>
    /// 
    /// </summary>
    Space = 4,
}
