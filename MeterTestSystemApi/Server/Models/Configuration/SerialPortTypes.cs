using System.Text.Json.Serialization;

namespace MeterTestSystemApi.Models.Configuration;

/// <summary>
/// All type of serial port connection.
/// </summary>
[JsonConverter(typeof(JsonStringEnumConverter))]
public enum SerialPortTypes
{
    /// <summary>
    /// RS232, e.g. /dev/ttyS1
    /// </summary>
    RS232 = 0,

    /// <summary>
    /// USB, e.g. /dev/ttyUSB0
    /// </summary>
    USB = 1,
}