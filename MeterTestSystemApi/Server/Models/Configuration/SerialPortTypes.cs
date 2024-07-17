using System.Text.Json.Serialization;

namespace MeterTestSystemApi.Models.Configuration;

/// <summary>
/// All type of serial port connection.
/// </summary>
[JsonConverter(typeof(JsonStringEnumConverter)), Flags]
public enum SerialPortTypes
{
    /// <summary>
    /// Skip this port.
    /// </summary>
    None = 0,

    /// <summary>
    /// RS232, e.g. /dev/ttyS1
    /// </summary>
    RS232 = 0x01,

    /// <summary>
    /// USB, e.g. /dev/ttyUSB0
    /// </summary>
    USB = 0x02,

    /// <summary>
    /// All possible types.
    /// </summary>
    All = RS232 | USB
}